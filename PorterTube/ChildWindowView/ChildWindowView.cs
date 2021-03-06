﻿using Porter.Entity;
using PorterTube.Common;
using PorterTube.Helper;
using PorterTube.View;
using PorterTube.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PorterTube.ChildWindowView
{
    public class ChildWindowView : BaseViewModel
    {
        public event Action<ObservableCollection<Porter.Entity.VideoDetails>> Closed;
        public ChildWindowView()
        {
            //var childWindow = new PorterTube.ChildWindowView.ChildWindowView();
            //childWindow.Closed += (r =>
            //{

            //    //RaisePropertyChanged("Address");
            //});
            //childWindow.Show("");
        }

        public void Show<T>(T obj)
        {
            string strObj = obj.ToString();
            if (!string.IsNullOrEmpty(strObj))
            {
                KeyValuePair<UrlType, string>? kvp = Porter.Model.Helper.ConvertStringToKeyValuePair(strObj);
                if (kvp.HasValue)
                {
                    ShowListVideoViewModel vm = new ShowListVideoViewModel(kvp.Value);
                    vm.Closed += ChildWindow_Closed;
                    ChildWindowManager.Instance.ShowChildWindow(new UCShowListVideo() { DataContext = vm });
                }
            }
        }

        void ChildWindow_Closed(ObservableCollection<Porter.Entity.VideoDetails> list)
        {
            if (Closed != null)
                Closed(list);
            ChildWindowManager.Instance.CloseChildWindow();
        }
    }
}
