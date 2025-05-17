using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspnetCoreMvcFull.Migrations
{
  /// <inheritdoc />
  public partial class addmodel : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      // Danh sách các cột cần thêm
      var columns = new[]
      {
                "aplucdaudunloithep",
                "caosubo",
                "caosuketdinh",
                "caosur514",
                "chieudai",
                "chieudai1",
                "chieudai2",
                "chieudailoithep",
                "doday1",
                "doday2",
                "dodaycaosubo",
                "dodaycaosuketdinh3t",
                "kho",
                "kho1",
                "kho2",
                "khoangcach2daumoinoibo",
                "khoangcach2daumoinoiloithep",
                "khocaosubo",
                "khocaosuketdinh3t",
                "kholoithep",
                "kichthuoccuacaosudanmoinoi",
                "loaicaosu",
                "loaicaosu1",
                "loaicaosu2",
                "loaikhuondun",
                "loaikhuondun1",
                "loaikhuondun2",
                "loithepsaukhidun",
                "loitheptruockhidun",
                "mahangctl",
                "nhietdodaumaydun",
                "nhietdotrucxoan",
                "solink",
                "sosoiloithep",
                "tocdocolingdrum",
                "tocdodun",
                "tocdoduncaosu",
                "tocdomotor",
                "tocdomotor1",
                "tocdomotor2",
                "tocdoquan",
                "trongluogtest",
                "trongluong",
                "trongluong1",
                "trongluong2",
                "trongluongloithepspinning"
            };

      // Tạo câu lệnh SQL để kiểm tra và thêm từng cột
      foreach (var column in columns)
      {
        migrationBuilder.Sql($@"
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Products' 
                        AND COLUMN_NAME = '{column}'
                    )
                    BEGIN
                        ALTER TABLE Products 
                        ADD [{column}] nvarchar(max) NULL;
                    END
                ");
      }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      // Danh sách các cột để xóa
      var columns = new[]
      {
                "aplucdaudunloithep",
                "caosubo",
                "caosuketdinh",
                "caosur514",
                "chieudai",
                "chieudai1",
                "chieudai2",
                "chieudailoithep",
                "doday1",
                "doday2",
                "dodaycaosubo",
                "dodaycaosuketdinh3t",
                "kho",
                "kho1",
                "kho2",
                "khoangcach2daumoinoibo",
                "khoangcach2daumoinoiloithep",
                "khocaosubo",
                "khocaosuketdinh3t",
                "kholoithep",
                "kichthuoccuacaosudanmoinoi",
                "loaicaosu",
                "loaicaosu1",
                "loaicaosu2",
                "loaikhuondun",
                "loaikhuondun1",
                "loaikhuondun2",
                "loithepsaukhidun",
                "loitheptruockhidun",
                "mahangctl",
                "nhietdodaumaydun",
                "nhietdotrucxoan",
                "solink",
                "sosoiloithep",
                "tocdocolingdrum",
                "tocdodun",
                "tocdoduncaosu",
                "tocdomotor",
                "tocdomotor1",
                "tocdomotor2",
                "tocdoquan",
                "trongluogtest",
                "trongluong",
                "trongluong1",
                "trongluong2",
                "trongluongloithepspinning"
            };

      // Xóa các cột nếu tồn tại
      foreach (var column in columns)
      {
        migrationBuilder.Sql($@"
                    IF EXISTS (
                        SELECT 1 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Products' 
                        AND COLUMN_NAME = '{column}'
                    )
                    BEGIN
                        ALTER TABLE Products 
                        DROP COLUMN [{column}];
                    END
                ");
      }
    }
  }
}
