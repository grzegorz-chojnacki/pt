using System.Globalization;
using System.Windows.Data;

namespace app {
    public class CultureResources {
        public Strings GetStringsInstance() {
            return new Strings();
        }
        private static ObjectDataProvider provider;
        public static ObjectDataProvider ResourceProvider {
            get {
                if (provider == null) {
                    provider = (ObjectDataProvider)System.Windows.Application
                        .Current.FindResource("Strings");
                }

                return provider;
            }
        }

        public static void ChangeCulture(CultureInfo culture) {
            ResourceProvider.Refresh();
        }
    }
}
