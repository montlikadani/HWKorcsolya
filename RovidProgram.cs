namespace Mukorcsolya {
    public class RovidProgram {

        public string Nev { get; private set; }
        public string Orszag { get; private set; }
        
        public double Technikai { get; private set; }
        public double Komponens { get; private set; }

        public int Levonas { get; private set; }

        public RovidProgram(string Nev, string Orszag, double Technikai, double Komponens, int Levonas) {
            this.Nev = Nev;
            this.Orszag = Orszag;
            this.Technikai = Technikai;
            this.Komponens = Komponens;
            this.Levonas = Levonas;
        }
    }
}
