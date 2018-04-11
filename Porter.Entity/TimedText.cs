using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porter.Entity
{
    /*
     https://www.youtube.com/api/timedtext?v=h0e2HAPTGF4&signature=3B444D0EE2F875B6E97CC030E647249F04462860.4DE22BCC86759D7EC7CABC7F25286E128F2F1BDC&xorp=True&expire=1521059980&hl=en_US&sparams=asr_langs%2Ccaps%2Cv%2Cxorp%2Cexpire&caps=asr&key=yttt1&asr_langs=fr%2Ces%2Cit%2Cpt%2Cen%2Cja%2Cnl%2Cde%2Cko%2Cru&lang=en&name=CC&fmt=srv3
     */

    public class Pen
    {
    }

    public class WsWinStyle
    {
    }

    public class WpWinPosition
    {
    }

    public class Seg
    {
        public string utf8 { get; set; }
    }

    public class Event
    {
        public int tStartMs { get; set; }
        public int dDurationMs { get; set; }
        public List<Seg> segs { get; set; }
    }

    public class TimedText
    {
        public string wireMagic { get; set; }
        public List<Pen> pens { get; set; }
        public List<WsWinStyle> wsWinStyles { get; set; }
        public List<WpWinPosition> wpWinPositions { get; set; }
        public List<Event> events { get; set; }
    }





}
