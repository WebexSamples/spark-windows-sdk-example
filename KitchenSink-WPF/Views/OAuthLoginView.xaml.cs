﻿#region License
// Copyright (c) 2016-2018 Cisco Systems, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using SparkSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KitchenSink
{
    public partial class OAuthLoginView : UserControl
    {

        OAuthLoginViewModel viewModel;
        public OAuthLoginView()
        {
            InitializeComponent();

            viewModel = this.DataContext as OAuthLoginViewModel;

            webBrowser.Navigate(viewModel.AuthorizationUrl);
            webBrowser.Navigating += new NavigatingCancelEventHandler(this.webBrowser_Navigating);
            webBrowser.LoadCompleted += new LoadCompletedEventHandler(this.webBrowser_LoadCompleted);

        }

        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            viewModel.TryToAnalyzeUrl(e.Uri.ToString());
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            SuppressScriptErrors(webBrowser, true);
            try
            {
                string title = ((dynamic)webBrowser.Document).Title as string;
                viewModel.TryToAnalyzeTitle(title);
            }
            catch
            {

            }

        }

        private void SuppressScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

    }
}
