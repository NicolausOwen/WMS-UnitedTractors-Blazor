using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class ProductService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly IWebHostEnvironment _env;

    public ProductService(IDbContextFactory<ApplicationDbContext> factory, IWebHostEnvironment env)
    {
        _factory = factory;
        _env = env;
    }

    public async Task<(List<Product> Products, int TotalItems, int TotalPages)> GetProductsAsync(int? category, string? search, int page = 1, int pageSize = 10, bool includeHidden = false)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .OrderByDescending(p => p.created_at)
            .AsQueryable();

        if (!includeHidden)
        {
            query = query.Where(p => p.is_hidden == 0);
        }

        if (category.HasValue)
        {
            query = query.Where(p => p.category_id == category.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.name.Contains(search) || p.sku.Contains(search));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalItems, totalPages);
    }

    public async Task<(List<Product> Products, int TotalItems, int TotalPages)> GetCatalogProductsAsync(
        int isReturnable, int? categoryId, string? search, int page = 1, int pageSize = 15)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Include(p => p.ProductVariants)
            .Where(p => p.is_returnable == isReturnable)
            .OrderBy(p => p.name)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.category_id == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.name.Contains(search) || p.sku.Contains(search));

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, total, totalPages);
    }

    public async Task<List<UT_WMSDotnet.Models.Category>> GetCategoriesForCatalogAsync(int isReturnable)
    {
        using var _context = _factory.CreateDbContext();
        var categoryIds = await _context.Products
            .Where(p => p.is_returnable == isReturnable && p.category_id != null)
            .Select(p => p.category_id!.Value)
            .Distinct()
            .ToListAsync();

        return await _context.Categories
            .Where(c => categoryIds.Contains(c.id))
            .OrderBy(c => c.name)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        return await _context.Products.Include(p => p.Unit).Include(p => p.ProductVariants).FirstOrDefaultAsync(p => p.id == id);
    }

    public async Task<string?> CreateProductAsync(ProductCreateViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.Products.AnyAsync(p => p.name == model.name))
        {
            return "Nama produk ini sudah ada di dalam sistem. Gunakan nama lain.";
        }

        if (!string.IsNullOrEmpty(model.sku) && await _context.Products.AnyAsync(p => p.sku == model.sku))
        {
            return "SKU atau Barcode ini sudah terdaftar pada produk lain.";
        }

        var unitName = model.unit.Trim().ToLower();
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.name.ToLower() == unitName);
        if (unit == null)
        {
            unit = new Unit { name = model.unit.Trim(), created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
        }

        var imagePaths = new List<string>();
        if (model.images != null && model.images.Count > 0)
        {
            var dirPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            foreach (var imgFile in model.images)
            {
                if (imgFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgFile.FileName);
                    var filePath = Path.Combine(dirPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    imagePaths.Add("/images/products/" + fileName);
                }
            }
        }
        else if (model.image != null && model.image.Length > 0)
        {
            var dirPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.image.FileName);
            using (var stream = new FileStream(Path.Combine(dirPath, fileName), FileMode.Create))
            {
                await model.image.CopyToAsync(stream);
            }
            imagePaths.Add("/images/products/" + fileName);
        }

        string? primaryImage = imagePaths.FirstOrDefault();
        string? imagesJson = imagePaths.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(imagePaths) : null;

        string? positionImagePath = null;
        if (model.position_image != null && model.position_image.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.position_image.FileName);
            var path = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await model.position_image.CopyToAsync(stream);
            }
            positionImagePath = "images/products/" + fileName;
        }

        var product = new Product
        {
            barcode_type = model.barcode_type,
            name = model.name,
            value = model.value ?? 0,
            category_id = model.category_id,
            location_id = model.location_id,
            unit_id = unit.id,
            is_returnable = model.is_returnable ? 1 : 0,
            initial_stock = model.initial_stock ?? 0,
            current_stock = model.initial_stock ?? 0,
            description = model.description,
            transaction_type = string.IsNullOrWhiteSpace(model.transaction_type) ? null : model.transaction_type,
            image = primaryImage,
            images = imagesJson,
            position_image = positionImagePath,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        if (string.IsNullOrEmpty(model.sku))
        {
            var count = await _context.Products.CountAsync();
            product.sku = $"UT-{DateTime.Now:yyMMdd}-{(count + 1):D4}";
        }
        else
        {
            product.sku = model.sku;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        if (model.variants != null && model.variants.Count > 0)
        {
            var variantsToAdd = new List<ProductVariant>();
            for (int i = 0; i < model.variants.Count; i++)
            {
                var v = model.variants[i];
                string? vImagePath = null;
                if (v.image != null && v.image.Size > 0)
                {
                    var vFileName = Guid.NewGuid().ToString() + Path.GetExtension(v.image.Name);
                    var vPath = Path.Combine(_env.WebRootPath, "images", "products", "variants");
                    if (!Directory.Exists(vPath)) Directory.CreateDirectory(vPath);
                    using (var stream = new FileStream(Path.Combine(vPath, vFileName), FileMode.Create))
                    {
                        await v.image.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);
                    }
                    vImagePath = "images/products/variants/" + vFileName;
                }

                variantsToAdd.Add(new ProductVariant
                {
                    product_id = product.id,
                    sku = string.IsNullOrEmpty(v.sku) ? $"{product.sku}-V{(i + 1):D2}" : v.sku,
                    color = v.color,
                    size = v.size,
                    stock = v.stock,
                    is_hidden = v.is_hidden ? 1 : 0,
                    image = vImagePath,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                });
            }
            _context.ProductVariants.AddRange(variantsToAdd);
            await _context.SaveChangesAsync();
        }

        return null;
    }

    public async Task<string?> UpdateProductAsync(int id, ProductUpdateViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        var product = await _context.Products.Include(p => p.ProductVariants).FirstOrDefaultAsync(p => p.id == id);
        if (product == null) return "Produk tidak ditemukan.";

        if (await _context.Products.AnyAsync(p => p.name == model.name && p.id != id))
        {
            return "Nama produk ini sudah ada di dalam sistem. Gunakan nama lain.";
        }

        if (await _context.Products.AnyAsync(p => p.sku == model.sku && p.id != id))
        {
            return "SKU atau Barcode ini sudah terdaftar pada produk lain.";
        }

        var unitName = model.unit.Trim().ToLower();
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.name.ToLower() == unitName);
        if (unit == null)
        {
            unit = new Unit { name = model.unit.Trim(), created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
        }

        List<string> currentImages = new List<string>();
        if (!string.IsNullOrEmpty(product.images))
        {
            try { currentImages = System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.images) ?? new List<string>(); }
            catch { /* ignore */ }
        }
        else if (!string.IsNullOrEmpty(product.image))
        {
            currentImages.Add(product.image);
        }

        if (model.removed_images != null && model.removed_images.Count > 0)
        {
            foreach (var remPath in model.removed_images)
            {
                var cleanPath = remPath.TrimStart('/');
                var pPath = Path.Combine(_env.WebRootPath, cleanPath);
                if (System.IO.File.Exists(pPath)) System.IO.File.Delete(pPath);
                currentImages.Remove(remPath);
            }
        }

        if (model.new_images != null && model.new_images.Count > 0)
        {
            var dirPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            foreach (var imgFile in model.new_images)
            {
                if (imgFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgFile.FileName);
                    var filePath = Path.Combine(dirPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    currentImages.Add("/images/products/" + fileName);
                }
            }
        }

        product.images = currentImages.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(currentImages) : null;
        product.image = currentImages.FirstOrDefault();

        if (model.remove_position_image)
        {
            if (!string.IsNullOrEmpty(product.position_image))
            {
                var path = Path.Combine(_env.WebRootPath, product.position_image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            product.position_image = null;
        }
        else if (model.position_image != null && model.position_image.Length > 0)
        {
            if (!string.IsNullOrEmpty(product.position_image))
            {
                var path = Path.Combine(_env.WebRootPath, product.position_image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.position_image.FileName);
            var dirPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            using (var stream = new FileStream(Path.Combine(dirPath, fileName), FileMode.Create))
            {
                await model.position_image.CopyToAsync(stream);
            }
            product.position_image = "images/products/" + fileName;
        }

        product.sku = model.sku;
        product.barcode_type = model.barcode_type;
        product.name = model.name;
        product.value = model.value ?? 0;
        product.category_id = model.category_id;
        product.location_id = model.location_id;
        product.unit_id = unit.id;
        product.current_stock = model.current_stock;
        product.initial_stock = model.initial_stock;
        product.is_returnable = model.is_returnable ? 1 : 0;
        product.is_hidden = model.is_hidden ? 1 : 0;
        product.description = model.description;
        product.transaction_type = string.IsNullOrWhiteSpace(model.transaction_type) ? null : model.transaction_type;
        product.updated_at = DateTime.UtcNow;

        _context.Update(product);

        // Update Variants
        var existingVariants = product.ProductVariants.ToList();
        var incomingVariantIds = model.variants.Where(v => v.id.HasValue).Select(v => v.id!.Value).ToList();

        // 1. Delete variants not in the incoming list
        var variantsToDelete = existingVariants.Where(v => !incomingVariantIds.Contains(v.id)).ToList();
        foreach (var v in variantsToDelete)
        {
            if (!string.IsNullOrEmpty(v.image))
            {
                var vPath = Path.Combine(_env.WebRootPath, v.image);
                if (System.IO.File.Exists(vPath)) System.IO.File.Delete(vPath);
            }
            _context.ProductVariants.Remove(v);
        }

        // 2. Update existing and Add new
        int newVariantCount = existingVariants.Count - variantsToDelete.Count;
        foreach (var vm in model.variants)
        {
            string? vImagePath = null;
            if (vm.image != null && vm.image.Size > 0)
            {
                var vFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.image.Name);
                var vDirPath = Path.Combine(_env.WebRootPath, "images", "products", "variants");
                if (!Directory.Exists(vDirPath)) Directory.CreateDirectory(vDirPath);
                using (var stream = new FileStream(Path.Combine(vDirPath, vFileName), FileMode.Create))
                {
                    await vm.image.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);
                }
                vImagePath = "images/products/variants/" + vFileName;
            }

            if (vm.id.HasValue && vm.id.Value > 0)
            {
                // Update
                var existing = existingVariants.FirstOrDefault(v => v.id == vm.id.Value);
                if (existing != null)
                {
                    existing.color = vm.color;
                    existing.size = vm.size;
                    existing.stock = vm.stock;
                    existing.is_hidden = vm.is_hidden ? 1 : 0;
                    existing.updated_at = DateTime.UtcNow;
                    if (vImagePath != null)
                    {
                        if (!string.IsNullOrEmpty(existing.image))
                        {
                            var oldVPath = Path.Combine(_env.WebRootPath, existing.image);
                            if (System.IO.File.Exists(oldVPath)) System.IO.File.Delete(oldVPath);
                        }
                        existing.image = vImagePath;
                    }
                    _context.ProductVariants.Update(existing);
                }
            }
            else
            {
                // Add
                newVariantCount++;
                var newVariant = new ProductVariant
                {
                    product_id = product.id,
                    sku = string.IsNullOrEmpty(vm.sku) ? $"{product.sku}-V{newVariantCount:D2}" : vm.sku,
                    color = vm.color,
                    size = vm.size,
                    stock = vm.stock,
                    is_hidden = vm.is_hidden ? 1 : 0,
                    image = vImagePath,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                _context.ProductVariants.Add(newVariant);
            }
        }

        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var product = await _context.Products.Include(p => p.ProductVariants).FirstOrDefaultAsync(p => p.id == id);
        if (product != null)
        {
            if (!string.IsNullOrEmpty(product.image))
            {
                var path = Path.Combine(_env.WebRootPath, product.image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            if (!string.IsNullOrEmpty(product.position_image))
            {
                var path = Path.Combine(_env.WebRootPath, product.position_image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            if (product.ProductVariants != null)
            {
                foreach (var variant in product.ProductVariants)
                {
                    if (!string.IsNullOrEmpty(variant.image))
                    {
                        var variantPath = Path.Combine(_env.WebRootPath, variant.image);
                        if (System.IO.File.Exists(variantPath)) System.IO.File.Delete(variantPath);
                    }
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<List<Product>> GetProductsByTypeAsync(string type, int id)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .AsQueryable();

        if (type == "category") query = query.Where(p => p.category_id == id);
        else if (type == "location") query = query.Where(p => p.location_id == id);
        else if (type == "unit") query = query.Where(p => p.unit_id == id);
        else if (type == "division")
        {
            var productIds = await _context.Transactions
                .Where(t => t.division_id == id)
                .Select(t => t.product_id)
                .Distinct()
                .ToListAsync();
            
            query = query.Where(p => productIds.Contains(p.id));
        }
        else
        {
            return new List<Product>();
        }

        return await query.OrderBy(p => p.name).ToListAsync();
    }

    public async Task<bool> ToggleProductVisibilityAsync(int id, bool isHidden)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.is_hidden = isHidden ? 1 : 0;
            product.updated_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> ToggleVariantVisibilityAsync(int variantId, bool isHidden)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant != null)
        {
            variant.is_hidden = isHidden ? 1 : 0;
            variant.updated_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
