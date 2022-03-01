using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting
{
    //This class contians the definitons of all the objects used in the project
    public partial class APIDefinitions
    {
        public string gecko_says { get; set; }
    }

    public partial class Exchange
    {
        public string Name { get; set; }
        public double open_interest_btc { get; set; }
        public string trade_volume_24h_btc { get; set; }
        public long number_of_perpetual_pairs { get; set; }
        public long number_of_futures_pairs { get; set; }
        public Uri Image { get; set; }
        public long year_established { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public string Error { get; set; }
    }

    public partial class Currency
    {
        public Dictionary<string, Rate> Rates { get; set; }
    }

    public partial class Rate
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }
    }

}
