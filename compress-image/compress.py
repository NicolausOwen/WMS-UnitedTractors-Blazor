import os
from PIL import Image

def compress_gallery(root_input_folder, root_output_folder, quality=60):
    valid_extensions = ('.jpg', '.jpeg', '.png', '.webp')
    
    # os.walk akan menelusuri seluruh folder dan sub-folder secara otomatis
    for dirpath, dirnames, filenames in os.walk(root_input_folder):
        
        # Membuat struktur folder hasil yang sama persis dengan folder asli
        relative_path = os.path.relpath(dirpath, root_input_folder)
        if relative_path == ".":
            current_output_dir = root_output_folder
        else:
            current_output_dir = os.path.join(root_output_folder, relative_path)
            
        if not os.path.exists(current_output_dir):
            os.makedirs(current_output_dir)

        # Proses setiap file gambar yang ditemukan
        for filename in filenames:
            if filename.lower().endswith(valid_extensions):
                input_path = os.path.join(dirpath, filename)
                
                # Mengubah ekstensi output menjadi .jpg agar hasil kompresi maksimal
                name_without_ext, _ = os.path.splitext(filename)
                output_filename = f"{name_without_ext}.jpg"
                output_path = os.path.join(current_output_dir, output_filename)
                
                try:
                    with Image.open(input_path) as img:
                        # PNG seringkali punya mode RGBA (transparan), 
                        # harus diubah ke RGB sebelum disimpan sebagai JPG
                        if img.mode in ("RGBA", "P"):
                            img = img.convert("RGB")
                            
                        # Simpan ke folder tujuan dengan optimasi JPG
                        img.save(output_path, "JPEG", optimize=True, quality=quality)
                        print(f"Berhasil: [{relative_path}] {filename} -> {output_filename}")
                except Exception as e:
                    print(f"Gagal memproses {filename}: {e}")

# --- ISI SESUAI JALUR FOLDER DI KOMPUTERMU ---
# Contoh jika foldermu ada di Drive D atau Desktop, sesuaikan path-nya:
FOLDER_ASLI = r"C:\Users\le\code\compress-image\Gambar Barang Corpu" 
FOLDER_HASIL_KOMPRES = r"C:\Users\le\code\compress-image\Gambar Barang Corpu_Compressed"

# Jalankan proses kompresi
compress_gallery(FOLDER_ASLI, FOLDER_HASIL_KOMPRES, quality=60)
print("\nHore! Semua folder dan gambar selesai dikompres!")