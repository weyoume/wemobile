using System;
using Foundation;

namespace Steepshot.iOS.Models
{
    public class CellSizeHelper
    {
        public nfloat PhotoHeight{ get; set; }
        public nfloat TextHeight { get; set; }
        public NSAttributedString Text { get; set; }
        public nfloat CellHeight => PhotoHeight + TextHeight + 192;

        public CellSizeHelper(nfloat PhotoHeight, nfloat TextHeight, NSAttributedString Text)
        {
            this.PhotoHeight = PhotoHeight;
            this.TextHeight = TextHeight;
            this.Text = Text;
        }
    }
}
