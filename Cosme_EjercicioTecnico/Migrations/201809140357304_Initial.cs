namespace Cosme_EjercicioTecnico.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExcepcionContable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        ProductFamily = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionCount = c.Int(nullable: false),
                        ReviewDate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExcepcionContable");
        }
    }
}
