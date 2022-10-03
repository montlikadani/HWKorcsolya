using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Controls;

namespace Mukorcsolya {
    public partial class MainWindow : Window {

        private readonly List<RovidProgram> rovidProgramok;
        private readonly List<Donto> dontok;

        public MainWindow() {
            InitializeComponent();

            IEnumerable<string> datas = File.ReadAllLines("rovidprogram.csv").Skip(1);

            rovidProgramok = new List<RovidProgram>(datas.Count());

            foreach (string one in datas) {
                string[] split = one.Split(';');

                rovidProgramok.Add(new RovidProgram(split[0], split[1],
                    ParseDouble(split[2]),
                    ParseDouble(split[3]), int.Parse(split[4])));
            }

            datas = File.ReadAllLines("donto.csv").Skip(1);
            dontok = new List<Donto>(datas.Count());

            foreach (string one in datas) {
                string[] split = one.Split(';');

                dontok.Add(new Donto(split[0], split[1], ParseDouble(split[2]), ParseDouble(split[3]), int.Parse(split[4])));
            }

            OsszesVersenyzo.Text = $"A rövidprogramban {rovidProgramok.Count()} induló volt";

            Donto mag = dontok.Find(d => d.Orszag.Equals("HUN"));

            MagyarDontos.Text = mag == null ? $"A magyar versenyző {mag.Nev} nem jutott be a kűrbe"
                : $"A magyar versenyző {mag.Nev} bejutott a kűrbe.";

            IOrderedEnumerable<Donto> ordered = dontok.OrderBy(d => d.Orszag);
            Donto last = null;

            foreach (Donto donto in ordered) {
                if (last != null && last.Orszag.Equals(donto.Orszag)) {
                    continue;
                }

                int amount = ordered.Count(one => one.Orszag.Equals(donto.Orszag));

                if (amount > 1) {
                    Osszesitesek.Items.Add($"{donto.Orszag}: {amount} versenyző");
                    last = donto;
                }
            }

            string vegeredmeny = "vegeredmeny.csv";

            if (File.Exists(vegeredmeny)) {
                File.Delete(vegeredmeny);
            }

            Dictionary<double, string> map = new Dictionary<double, string>(dontok.Count);

            foreach (Donto one in dontok) {
                Donto vegs = dontok.Find(ds => ds.Nev.Equals(one.Nev));
                RovidProgram rp = rovidProgramok.Find(r => r.Nev.Equals(one.Nev));

                if (vegs != null && rp != null) {
                    double er = rp.Komponens + rp.Technikai + vegs.Komponens + vegs.Technikai;

                    map.Add(er, $"{vegs.Nev};{vegs.Orszag};{er}");
                }
            }

            int i = 0;

            using (StreamWriter writer = File.CreateText(vegeredmeny)) {
                foreach (KeyValuePair<double, string> pair in from entry in map orderby entry.Key descending select entry) {
                    writer.WriteLine($"{++i};" + pair.Value);
                }
            }
        }

        private double ParseDouble(string input) {
            return double.TryParse(input.Replace('.', ','), out double res) ? res : 0;
        }

        private double OsszPontszam(RovidProgram vers) {
            Donto dontos = dontok.Find(ds => ds.Nev.Equals(vers.Nev));

            return dontos == null ? vers.Technikai : dontos.Technikai + vers.Technikai;
        }

        private void NevInput_TextChanged(object sender, TextChangedEventArgs e) {
            DontRes.Visibility = sixthTask.Visibility = Visibility.Visible;
            string nev = NevInput.Text;

            RovidProgram indulo = rovidProgramok.Find(rp => rp.Nev.Equals(nev));

            DontRes.Text = indulo == null ? "Ilyen nevű induló nem volt" : $"Versenyző összpontszáma: {OsszPontszam(indulo)}";
        }
    }
}
