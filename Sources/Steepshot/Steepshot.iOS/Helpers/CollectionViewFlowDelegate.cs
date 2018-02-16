﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Enums;
using Steepshot.Core.Presenters;
using Steepshot.iOS.Models;
using UIKit;

namespace Steepshot.iOS.Helpers
{
    public class CollectionViewFlowDelegate : UICollectionViewDelegateFlowLayout
    {
        public Action ScrolledToBottom;
        public Action<ActionType, Post> CellClicked;
        public bool IsGrid = true;
        private BasePostPresenter _presenter;
        private UICollectionView _collection;
        private int _prevPos;
        public List<CellSizeHelper> Variables = new List<CellSizeHelper>();

        public int Position => _prevPos;
        public NSIndexPath TopCurrentPosition;

        public void ClearPosition()
        {
            _prevPos = 0;
            Variables.Clear();
        }

        public CollectionViewFlowDelegate(UICollectionView collection, BasePostPresenter presenter = null)
        {
            _collection = collection;
            _presenter = presenter;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (_collection.IndexPathsForVisibleItems.Length > 0)
            {
                var pos = _collection.IndexPathsForVisibleItems.Max(c => c.Row);
                //TopCurrentPosition = _collection.IndexPathsForVisibleItems.Min();
                if (pos > _prevPos)
                {
                    if (pos == _presenter.Count - 1)
                    {
                        _prevPos = pos;
                        ScrolledToBottom?.Invoke();
                    }
                }
            }
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (!IsGrid)
                return;

            CellClicked?.Invoke(ActionType.Preview, _presenter[(int)indexPath.Item]);
        }

        public void GenerateVariables()
        {
            if (Variables.Count == _presenter.Count)
                return;
            for (int i = Variables.Count; i < _presenter.Count; i++)
            {
                var cellVariables = CellHeightCalculator.Calculate(_presenter[i]);
                Variables.Add(cellVariables);
            }
        }

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            if (Variables.Count > indexPath.Item)
            {
                if (IsGrid)
                    return Constants.CellSize;
                return new CGSize(UIScreen.MainScreen.Bounds.Width, Variables[(int)indexPath.Item].CellHeight);
            }
            //loader height
            return new CGSize(UIScreen.MainScreen.Bounds.Width, 80);
        } 
    }
}
