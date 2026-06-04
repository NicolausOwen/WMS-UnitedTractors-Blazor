using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        var sourcePath = @"C:\Users\User\source\repos\WMS-UnitedTracors-Blazor\WMS-UnitedTracors-Blazor\Migrations\Seeders\wms-ut.sql";
        var destPath = @"C:\Users\User\source\repos\WMS-UnitedTracors-Blazor\WMS-UnitedTracors-Blazor\Migrations\Seeders\wms_united_tractors_clean.sql";
        
        var lines = File.ReadAllLines(sourcePath);
        
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("-- ------------------------------------------------------------");
        sb.AppendLine("-- Table: transactions (Imported from wms-ut.sql)");
        sb.AppendLine("-- ------------------------------------------------------------");
        sb.AppendLine();
        
        // CREATE TABLE
        for(int i = 756; i <= 791; i++) sb.AppendLine(lines[i]);
        
        sb.AppendLine();
        
        // INSERT
        for(int i = 797; i <= 808; i++) sb.AppendLine(lines[i]);
        
        sb.AppendLine();
        
        // ALTER TABLE PRIMARY KEY
        for(int i = 1006; i <= 1012; i++) sb.AppendLine(lines[i]);
        
        sb.AppendLine();
        
        // AUTO_INCREMENT
        for(int i = 1097; i <= 1098; i++) sb.AppendLine(lines[i]);
        
        sb.AppendLine();
        
        // CONSTRAINTS
        for(int i = 1147; i <= 1152; i++) sb.AppendLine(lines[i]);
        
        File.AppendAllText(destPath, sb.ToString());
    }
}
