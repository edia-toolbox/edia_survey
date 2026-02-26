using System.Collections.Generic;
using UnityEngine;

namespace Edia.Survey {
    public static class Constants {
        public enum IndicatorTypes {
            None,
            Numbers,
            Lines,
            Bands
        }
        
        public enum VasTypes {
            Slider,
            Markers
        }

        public enum UnitSymbols {
            None,
            Percent, // %
            Dollar, // $
            Euro // €
        }
        public static readonly List<string> UnitSymbolsLookup = new List<string> {
            " ",
            "%",
            "$",
            "€"
        };
    }
}