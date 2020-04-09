using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class MainVM {
        private MapVM mapVM { get; set; }
        private ConnectionButtonsVM connectionBtnVM { get; set; }
        private CockpitDashboardVM cockpitVM { get; set; }

        public MainVM(SimulatorModel simModel) {
            mapVM = new MapVM(simModel);
        }
    }
}