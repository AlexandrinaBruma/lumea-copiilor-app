using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace LumeaCopiilor
{

    public class ProdusStatistica
    {
        public string Nume      { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public int    TotalVandut { get; set; }
    }

    public class UtilizatorTop
    {
        public string NumeComplet { get; set; } = string.Empty;
        public string Email       { get; set; } = string.Empty;
        public int    NrComenzi   { get; set; }
    }

    public class AdminInfo
    {
        public string NumeUtilizator { get; set; } = string.Empty;
        public string NumeComplet    { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
    }


    public class CumparatureExport
    {
        public string  NumeProdus    { get; set; } = string.Empty;
        public string  NumeClient    { get; set; } = string.Empty;
        public int     Cantitate     { get; set; }
        public decimal PretUnitar    { get; set; }
        public decimal TotalPret     { get; set; }
        public string  DataCumparare { get; set; } = string.Empty;
        public string  Magazin       { get; set; } = string.Empty;
        public string  TipPlata      { get; set; } = string.Empty;
    }

    public class ProdusExport
    {
        public string  Nume          { get; set; } = string.Empty;
        public string  Categorie     { get; set; } = string.Empty;
        public decimal Pret          { get; set; }
        public int     Cantitate     { get; set; }
        public int     VarstaMin     { get; set; }
        public int     VarstaMax     { get; set; }
        public string  DataFabricatie { get; set; } = string.Empty;
        public string  DataExpirare  { get; set; } = string.Empty;
        public string  TaraOrigine   { get; set; } = string.Empty;
        public string  Importator    { get; set; } = string.Empty;
    }

    public class FaraStocExport
    {
        public string  Nume          { get; set; } = string.Empty;
        public decimal Pret          { get; set; }
        public int     VarstaMin     { get; set; }
        public int     VarstaMax     { get; set; }
        public string  Categorie     { get; set; } = string.Empty;
        public string  TaraOrigine   { get; set; } = string.Empty;
        public string  Importator    { get; set; } = string.Empty;
        public string  DataArhivare  { get; set; } = string.Empty;
    }

    public partial class StatisticiWindow : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        public StatisticiWindow()
        {
            InitializeComponent();
            LoadProdusePopulare();
            LoadTopUtilizatori();
            LoadAdministratori();
            LoadProduseNepopulare();
        }


        private void LoadProdusePopulare()
        {
            var lista = new List<ProdusStatistica>();
            try
            {
                const string sql = @"
                    SELECT TOP 10
                        p.Name          AS Nume,
                        c.Name          AS Categorie,
                        SUM(pu.Quantity) AS TotalVandut
                    FROM Purchase pu
                    JOIN Product  p  ON pu.Product  = p.ProductID
                    JOIN Category c  ON p.Category  = c.CategoryID
                    GROUP BY p.ProductID, p.Name, c.Name
                    ORDER BY TotalVandut DESC";

                using SqlConnection conn = new(ConnectionString);
                using SqlCommand    cmd  = new(sql, conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    lista.Add(new ProdusStatistica
                    {
                        Nume        = r["Nume"].ToString()!,
                        Categorie   = r["Categorie"].ToString()!,
                        TotalVandut = Convert.ToInt32(r["TotalVandut"])
                    });
            }
            catch { }

            ProdusePopulareGrid.ItemsSource = lista;
        }

        private void LoadTopUtilizatori()
        {
            var lista = new List<UtilizatorTop>();
            try
            {
                const string sql = @"
                    SELECT TOP 10
                        u.Name + ' ' + u.Surname AS NumeComplet,
                        u.Email,
                        COUNT(*) AS NrComenzi
                    FROM Purchase pu
                    JOIN Utilizator u ON pu.Client = u.UtilizatorID
                    GROUP BY u.UtilizatorID, u.Name, u.Surname, u.Email
                    ORDER BY NrComenzi DESC";

                using SqlConnection conn = new(ConnectionString);
                using SqlCommand    cmd  = new(sql, conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    lista.Add(new UtilizatorTop
                    {
                        NumeComplet = r["NumeComplet"].ToString()!,
                        Email       = r["Email"].ToString()!,
                        NrComenzi   = Convert.ToInt32(r["NrComenzi"])
                    });
            }
            catch { }

            TopUtilizatoriGrid.ItemsSource = lista;
        }

        private void LoadAdministratori()
        {
            var lista = new List<AdminInfo>();
            try
            {
                const string sql = @"
                    SELECT Username,
                           Name + ' ' + Surname AS NumeComplet,
                           Email
                    FROM Utilizator
                    WHERE Role = 'A'
                    ORDER BY Name, Surname";

                using SqlConnection conn = new(ConnectionString);
                using SqlCommand    cmd  = new(sql, conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    lista.Add(new AdminInfo
                    {
                        NumeUtilizator = r["Username"].ToString()!,
                        NumeComplet    = r["NumeComplet"].ToString()!,
                        Email          = r["Email"].ToString()!
                    });
            }
            catch { }

            AdministratoriGrid.ItemsSource = lista;
        }

        private void LoadProduseNepopulare()
        {
            var lista = new List<ProdusStatistica>();
            try
            {
                const string sql = @"
                    SELECT TOP 10
                        p.Name           AS Nume,
                        c.Name           AS Categorie,
                        SUM(pu.Quantity) AS TotalVandut
                    FROM Purchase pu
                    JOIN Product  p  ON pu.Product = p.ProductID
                    JOIN Category c  ON p.Category = c.CategoryID
                    GROUP BY p.ProductID, p.Name, c.Name
                    ORDER BY TotalVandut ASC";

                using SqlConnection conn = new(ConnectionString);
                using SqlCommand    cmd  = new(sql, conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    lista.Add(new ProdusStatistica
                    {
                        Nume        = r["Nume"].ToString()!,
                        Categorie   = r["Categorie"].ToString()!,
                        TotalVandut = Convert.ToInt32(r["TotalVandut"])
                    });
            }
            catch { }

            ProduseNepopulareGrid.ItemsSource = lista;
        }

        private List<CumparatureExport> GetToateCumparaturile()
        {
            var lista = new List<CumparatureExport>();
            const string sql = @"
                SELECT
                    p.Name                                              AS NumeProdus,
                    u.Name + ' ' + u.Surname                           AS NumeClient,
                    pu.Quantity                                         AS Cantitate,
                    p.Price                                             AS PretUnitar,
                    p.Price * pu.Quantity                              AS TotalPret,
                    FORMAT(pu.Purchase_date, 'dd/MM/yyyy HH:mm')       AS DataCumparare,
                    s.Street_address + ', ' + ci.Name                  AS Magazin,
                    pay.Type                                            AS TipPlata
                FROM Purchase   pu
                JOIN Product    p   ON pu.Product      = p.ProductID
                JOIN Utilizator u   ON pu.Client       = u.UtilizatorID
                JOIN Shop       s   ON pu.Shop         = s.ShopID
                JOIN City       ci  ON s.City          = ci.CityID
                JOIN Payment    pay ON pu.Payment_type = pay.PaymentID
                ORDER BY pu.Purchase_date DESC";

            using SqlConnection conn = new(ConnectionString);
            using SqlCommand    cmd  = new(sql, conn);
            conn.Open();
            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new CumparatureExport
                {
                    NumeProdus    = r["NumeProdus"].ToString()!,
                    NumeClient    = r["NumeClient"].ToString()!,
                    Cantitate     = Convert.ToInt32(r["Cantitate"]),
                    PretUnitar    = Convert.ToDecimal(r["PretUnitar"]),
                    TotalPret     = Convert.ToDecimal(r["TotalPret"]),
                    DataCumparare = r["DataCumparare"].ToString()!,
                    Magazin       = r["Magazin"].ToString()!,
                    TipPlata      = r["TipPlata"].ToString()!
                });

            return lista;
        }

        private List<ProdusExport> GetToateProdusele()
        {
            var lista = new List<ProdusExport>();
            const string sql = @"
                SELECT
                    p.Name                                              AS Nume,
                    ISNULL(cat.Name,       '')                          AS Categorie,
                    ISNULL(p.Price,        0)                           AS Pret,
                    ISNULL(p.Quantity,     0)                           AS Cantitate,
                    ISNULL(p.Min_age,      0)                           AS VarstaMin,
                    ISNULL(p.Max_age,      0)                           AS VarstaMax,
                    ISNULL(FORMAT(p.Fab_date, 'dd/MM/yyyy'), '')        AS DataFabricatie,
                    ISNULL(FORMAT(p.Exp_date, 'dd/MM/yyyy'), '')        AS DataExpirare,
                    ISNULL(co.Name,        '')                          AS TaraOrigine,
                    ISNULL(imp.Company_name, '')                        AS Importator
                FROM Product    p
                LEFT JOIN Category   cat ON p.Category       = cat.CategoryID
                LEFT JOIN Country    co  ON p.Origin_country = co.CountryID
                LEFT JOIN Importator imp ON p.Importator     = imp.ImportatorID
                ORDER BY cat.Name, p.Name";

            using SqlConnection conn = new(ConnectionString);
            using SqlCommand    cmd  = new(sql, conn);
            conn.Open();
            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new ProdusExport
                {
                    Nume           = r["Nume"]?.ToString() ?? string.Empty,
                    Categorie      = r["Categorie"]?.ToString() ?? string.Empty,
                    Pret           = r["Pret"]          == DBNull.Value ? 0m : Convert.ToDecimal(r["Pret"]),
                    Cantitate      = r["Cantitate"]     == DBNull.Value ? 0  : Convert.ToInt32(r["Cantitate"]),
                    VarstaMin      = r["VarstaMin"]     == DBNull.Value ? 0  : Convert.ToInt32(r["VarstaMin"]),
                    VarstaMax      = r["VarstaMax"]     == DBNull.Value ? 0  : Convert.ToInt32(r["VarstaMax"]),
                    DataFabricatie = r["DataFabricatie"]?.ToString() ?? string.Empty,
                    DataExpirare   = r["DataExpirare"]?.ToString()   ?? string.Empty,
                    TaraOrigine    = r["TaraOrigine"]?.ToString()    ?? string.Empty,
                    Importator     = r["Importator"]?.ToString()     ?? string.Empty
                });

            return lista;
        }

        private List<FaraStocExport> GetProduseFaraStoc()
        {
            var lista = new List<FaraStocExport>();
            const string sql = @"
                SELECT
                    o.Name                                              AS Nume,
                    ISNULL(o.Price,    0)                               AS Pret,
                    ISNULL(o.Min_age,  0)                               AS VarstaMin,
                    ISNULL(o.Max_age,  0)                               AS VarstaMax,
                    ISNULL(c.Name,     '')                              AS Categorie,
                    ISNULL(co.Name,    '')                              AS TaraOrigine,
                    ISNULL(imp.Company_name, '')                        AS Importator,
                    ISNULL(FORMAT(o.Archived_date, 'dd/MM/yyyy HH:mm'), '') AS DataArhivare
                FROM OutOfStock o
                LEFT JOIN Category   c   ON o.Category       = c.CategoryID
                LEFT JOIN Country    co  ON o.Origin_country = co.CountryID
                LEFT JOIN Importator imp ON o.Importator     = imp.ImportatorID
                ORDER BY o.Archived_date DESC";

            using SqlConnection conn = new(ConnectionString);
            using SqlCommand    cmd  = new(sql, conn);
            conn.Open();
            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new FaraStocExport
                {
                    Nume         = r["Nume"]?.ToString()         ?? string.Empty,
                    Pret         = r["Pret"]      == DBNull.Value ? 0m : Convert.ToDecimal(r["Pret"]),
                    VarstaMin    = r["VarstaMin"] == DBNull.Value ? 0  : Convert.ToInt32(r["VarstaMin"]),
                    VarstaMax    = r["VarstaMax"] == DBNull.Value ? 0  : Convert.ToInt32(r["VarstaMax"]),
                    Categorie    = r["Categorie"]?.ToString()    ?? string.Empty,
                    TaraOrigine  = r["TaraOrigine"]?.ToString()  ?? string.Empty,
                    Importator   = r["Importator"]?.ToString()   ?? string.Empty,
                    DataArhivare = r["DataArhivare"]?.ToString() ?? string.Empty
                });

            return lista;
        }

        private void ExportCumparaturiWord_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title      = "Salvați raportul Word",
                Filter     = "Document Word (*.docx)|*.docx",
                FileName   = $"Cumparaturi_{DateTime.Now:yyyyMMdd_HHmm}"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var cumparaturi = GetToateCumparaturile();

                using WordprocessingDocument doc =
                    WordprocessingDocument.Create(dlg.FileName, WordprocessingDocumentType.Document);

                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                body.AppendChild(BuildWordParagraph(
                    $"Raport Cumpărături — Lumea Copiilor  |  {DateTime.Now:dd/MM/yyyy HH:mm}",
                    bold: true, fontSize: 28));

                body.AppendChild(new Paragraph()); 

                string[] anteturi = { "Produs", "Client", "Cantitate", "Preț unitar", "Total", "Data", "Magazin", "Tip plată" };

                Table tabel = new Table();
                tabel.AppendChild(BuildWordTableBorders());
                tabel.AppendChild(BuildWordHeaderRow(anteturi));

                foreach (var c in cumparaturi)
                {
                    TableRow rand = new TableRow();
                    rand.AppendChild(BuildWordCell(c.NumeProdus));
                    rand.AppendChild(BuildWordCell(c.NumeClient));
                    rand.AppendChild(BuildWordCell(c.Cantitate.ToString()));
                    rand.AppendChild(BuildWordCell($"{c.PretUnitar:0.00} lei"));
                    rand.AppendChild(BuildWordCell($"{c.TotalPret:0.00} lei"));
                    rand.AppendChild(BuildWordCell(c.DataCumparare));
                    rand.AppendChild(BuildWordCell(c.Magazin));
                    rand.AppendChild(BuildWordCell(c.TipPlata));
                    tabel.AppendChild(rand);
                }

                body.AppendChild(tabel);
                mainPart.Document.Save();

                MessageBox.Show(
                    $"Raportul Word a fost exportat cu succes!\n({cumparaturi.Count} înregistrări)",
                    "Export reușit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la exportul Word:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportCumparaturiExcel_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title    = "Salvați raportul Excel",
                Filter   = "Registru Excel (*.xlsx)|*.xlsx",
                FileName = $"Cumparaturi_{DateTime.Now:yyyyMMdd_HHmm}"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var cumparaturi = GetToateCumparaturile();

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Cumpărături");

                string[] capete = { "Produs", "Client", "Cantitate", "Preț unitar (lei)", "Total (lei)", "Data cumpărării", "Magazin", "Tip plată" };
                for (int col = 0; col < capete.Length; col++)
                {
                    var cell = ws.Cell(1, col + 1);
                    cell.Value = capete[col];
                    StyleHeaderCell(cell);
                }

                for (int i = 0; i < cumparaturi.Count; i++)
                {
                    var c   = cumparaturi[i];
                    int row = i + 2;
                    ws.Cell(row, 1).Value = c.NumeProdus;
                    ws.Cell(row, 2).Value = c.NumeClient;
                    ws.Cell(row, 3).Value = c.Cantitate;
                    ws.Cell(row, 4).Value = c.PretUnitar;
                    ws.Cell(row, 5).Value = c.TotalPret;
                    ws.Cell(row, 6).Value = c.DataCumparare;
                    ws.Cell(row, 7).Value = c.Magazin;
                    ws.Cell(row, 8).Value = c.TipPlata;

                    // Alternating row color
                    if (i % 2 == 0)
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF5F0");
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);

                MessageBox.Show(
                    $"Raportul Excel a fost exportat cu succes!\n({cumparaturi.Count} înregistrări)",
                    "Export reușit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la exportul Excel:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportProduseExcel_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title    = "Salvați lista produselor",
                Filter   = "Registru Excel (*.xlsx)|*.xlsx",
                FileName = $"Produse_{DateTime.Now:yyyyMMdd_HHmm}"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var produse = GetToateProdusele();

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Produse");

                string[] capete =
                {
                    "Denumire", "Categorie", "Preț (lei)", "Stoc (buc.)",
                    "Vârstă min.", "Vârstă max.", "Data fabricației",
                    "Data expirării", "Țara de origine", "Importator"
                };

                for (int col = 0; col < capete.Length; col++)
                {
                    var cell = ws.Cell(1, col + 1);
                    cell.Value = capete[col];
                    StyleHeaderCell(cell);
                }

                for (int i = 0; i < produse.Count; i++)
                {
                    var p   = produse[i];
                    int row = i + 2;
                    ws.Cell(row, 1).Value  = p.Nume;
                    ws.Cell(row, 2).Value  = p.Categorie;
                    ws.Cell(row, 3).Value  = p.Pret;
                    ws.Cell(row, 4).Value  = p.Cantitate;
                    ws.Cell(row, 5).Value  = p.VarstaMin;
                    ws.Cell(row, 6).Value  = p.VarstaMax;
                    ws.Cell(row, 7).Value  = p.DataFabricatie;
                    ws.Cell(row, 8).Value  = p.DataExpirare;
                    ws.Cell(row, 9).Value  = p.TaraOrigine;
                    ws.Cell(row, 10).Value = p.Importator;

                    if (i % 2 == 0)
                        ws.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF5F0");
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);

                MessageBox.Show(
                    $"Lista produselor a fost exportată cu succes!\n({produse.Count} produse)",
                    "Export reușit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la exportul Excel:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportFaraStocExcel_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title    = "Salvați produsele fără stoc",
                Filter   = "Registru Excel (*.xlsx)|*.xlsx",
                FileName = $"ProduseFaraStoc_{DateTime.Now:yyyyMMdd_HHmm}"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var faraStoc = GetProduseFaraStoc();

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Fără stoc");

                string[] capete =
                {
                    "Denumire", "Preț (lei)", "Vârstă min.", "Vârstă max.",
                    "Categorie", "Țara de origine", "Importator", "Data arhivării"
                };

                for (int col = 0; col < capete.Length; col++)
                {
                    var cell = ws.Cell(1, col + 1);
                    cell.Value = capete[col];
                    StyleHeaderCell(cell, headerColor: "#C0392B"); 
                }

                for (int i = 0; i < faraStoc.Count; i++)
                {
                    var f   = faraStoc[i];
                    int row = i + 2;
                    ws.Cell(row, 1).Value = f.Nume;
                    ws.Cell(row, 2).Value = f.Pret;
                    ws.Cell(row, 3).Value = f.VarstaMin;
                    ws.Cell(row, 4).Value = f.VarstaMax;
                    ws.Cell(row, 5).Value = f.Categorie;
                    ws.Cell(row, 6).Value = f.TaraOrigine;
                    ws.Cell(row, 7).Value = f.Importator;
                    ws.Cell(row, 8).Value = f.DataArhivare;

                    if (i % 2 == 0)
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF0F0");
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);

                MessageBox.Show(
                    $"Produsele fără stoc au fost exportate cu succes!\n({faraStoc.Count} produse)",
                    "Export reușit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la exportul Excel:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            DashboardAdmin dashboard = new DashboardAdmin();
            dashboard.Show();
            this.Close();
        }

        private static void StyleHeaderCell(IXLCell cell, string headerColor = "#AC9292")
        {
            cell.Style.Font.Bold            = true;
            cell.Style.Font.FontColor       = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(headerColor);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private static TableProperties BuildWordTableBorders()
        {
            return new TableProperties(
                new TableBorders(
                    new TopBorder            { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder         { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder           { Val = BorderValues.Single, Size = 4 },
                    new RightBorder          { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                ),
                new TableWidth { Width = "9638", Type = TableWidthUnitValues.Dxa }
            );
        }

        private static TableRow BuildWordHeaderRow(string[] headers)
        {
            TableRow rand = new TableRow();
            foreach (string h in headers)
            {
                TableCell celula = new TableCell(
                    new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Auto },
                        new Shading { Fill = "AC9292", Color = "auto", Val = ShadingPatternValues.Clear }
                    ),
                    new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(new Bold(), new Color { Val = "FFFFF8" },
                                              new FontSize { Val = "20" }),
                            new Text(h)
                        )
                    )
                );
                rand.AppendChild(celula);
            }
            return rand;
        }

        private static TableCell BuildWordCell(string text)
        {
            return new TableCell(
                new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }),
                new Paragraph(
                    new Run(
                        new RunProperties(new FontSize { Val = "18" }),
                        new Text(text ?? string.Empty)
                    )
                )
            );
        }

        private static Paragraph BuildWordParagraph(string text, bool bold = false, int fontSize = 22)
        {
            RunProperties rp = new RunProperties(new FontSize { Val = fontSize.ToString() });
            if (bold) rp.AppendChild(new Bold());

            return new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines { After = "160" }),
                new Run(rp, new Text(text))
            );
        }
    }
}
