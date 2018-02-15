using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using Steepshot.Core.Localization;
using Steepshot.Core.Models;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Utils;
using Steepshot.iOS.Models;
using UIKit;
using Xamarin.TTTAttributedLabel;

namespace Steepshot.iOS.Helpers
{
    public static class CellHeightCalculator
    {
        private static readonly UIStringAttributes _noLinkAttribute;

        static CellHeightCalculator()
        {
            _noLinkAttribute = new UIStringAttributes
            {
                Font = Constants.Regular14,
                ForegroundColor = Constants.R15G24B30,
            };
        }

        public static CellSizeHelper Calculate(Post post)
        {
            var attributedLabel = new TTTAttributedLabel();
            var at = new NSMutableAttributedString();
            var photoHeight = (int)(OptimalPhotoSize.Get(new Size() { Height = post.Media[0].Size.Height, Width = post.Media[0].Size.Width }, 
                                                         (float)UIScreen.MainScreen.Bounds.Width, 180, (float)UIScreen.MainScreen.Bounds.Width + 50));

            at.Append(new NSAttributedString(post.Title, _noLinkAttribute));
            if (!string.IsNullOrEmpty(post.Description))
            {
                at.Append(new NSAttributedString(Environment.NewLine));
                at.Append(new NSAttributedString(Environment.NewLine));
                at.Append(new NSAttributedString(post.Description, _noLinkAttribute));
            }

            foreach (var tag in post.Tags)
            {
                if (tag == "steepshot")
                    continue;
                var linkAttribute = new UIStringAttributes
                {
                    Link = new NSUrl(tag),
                    Font = Constants.Regular14,
                    ForegroundColor = Constants.R231G72B0,
                };
                at.Append(new NSAttributedString($" ", _noLinkAttribute));
                at.Append(new NSAttributedString($"#{tag}", linkAttribute));
            }

            attributedLabel.Lines = 0;
            attributedLabel.SetText(at);

            /*

            for (nuint numberOfLines = 0; numberOfLines < numberOfGlyphs; numberOfLines++)
            {
                layoutManager.LineFragmentRectForGlyphAtIndex(index, ref lineRange);
                layoutManager.LineFragmentRectForGlyphAtIndex(index + (nuint)lineRange.Length, ref lineRange);
                layoutManager.LineFragmentRectForGlyphAtIndex(index + (nuint)lineRange.Length + 1, ref lineRange);
                var h = lineRange.Length;
            }*/
            //for (numberOfLines, index; index < numberOfGlyphs; numberOfLines++)
            //{
                
            //}

            var textHeight = attributedLabel.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Width - 15 * 2, 0)).Height;

            var t = new UITextView();
            //var t = new UITextView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 15 * 2, textHeight));
            t.AttributedText = at;
            var y = t.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Width - 15 * 2, 0));
            t.Frame = new CGRect(new CGPoint(0, 0), y);

            var layoutManager = t.LayoutManager;
            nuint numberOfLines = 0;
            nuint index = 0;
            var lineRange = new NSRange();
            var numberOfGlyphs = layoutManager.NumberOfGlyphs;

            var ty = new UIStringAttributes();

            //var u = new NSString("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            //u.GetBoundingRect(new CGSize(UIScreen.MainScreen.Bounds.Width - 15 * 2, 40), NSStringDrawingOptions.UsesLineFragmentOrigin, , null);

            while (numberOfGlyphs > index)
            {
                numberOfLines++;

                if (numberOfLines == 6)
                {
                    UITextPosition beginning = t.BeginningOfDocument;
                    UITextPosition from = t.GetPosition(beginning, (nint)lineRange.Location);
                    UITextPosition to = t.GetPosition(beginning, (nint)index);

                    var r = t.GetTextRange(from, to);
                    var wordToInsert = AppSettings.LocalizationManager.GetText(LocalizationKeys.ShowMoreString);
                    var lineToTruncate = t.TextInRange(r);
                    var charArray = lineToTruncate.ToCharArray();
                    bool zashlo = false;

                    var textViewForLineLengthCheck = new UITextView();
                    textViewForLineLengthCheck.Font = Constants.Regular14;
                    var clearedLine = lineToTruncate.Remove(lineToTruncate.Length - 1, 1);
                    textViewForLineLengthCheck.Text = clearedLine + wordToInsert;
                    var yg = textViewForLineLengthCheck.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Width - 15 * 2, 35));

                    //35.5 - textview height with 14regular font
                    if (yg.Height > 36)
                    {
                        for (int i = charArray.Length - wordToInsert.Length; i >= 0; i--)
                        {
                            if (charArray[i] == ' ')
                            {
                                from = t.GetPosition(beginning, (nint)lineRange.Location + i);
                                to = t.GetPosition(beginning, (nint)numberOfGlyphs);
                                r = t.GetTextRange(from, to);
                                zashlo = true;
                                break;
                            }
                        }

                        if (!zashlo)
                        {
                            to = t.GetPosition(beginning, (nint)numberOfGlyphs);
                            r = t.GetTextRange(from, to);
                            wordToInsert = wordToInsert.Trim();
                        }
                    }
                    else
                    {
                        to = t.GetPosition(beginning, (nint)numberOfGlyphs);
                        r = t.GetTextRange(from, to);

                        wordToInsert = textViewForLineLengthCheck.Text;
                    }

                    t.ReplaceText(r, wordToInsert);
                    break;
                }
                layoutManager.LineFragmentRectForGlyphAtIndex(index, ref lineRange);
                index = (nuint)(lineRange.Length + lineRange.Location);

            }
            attributedLabel.SetText(t.AttributedText);
            textHeight = attributedLabel.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Width - 15 * 2, 0)).Height;

            return new CellSizeHelper(photoHeight, textHeight, t.AttributedText);
        }
    }
}
