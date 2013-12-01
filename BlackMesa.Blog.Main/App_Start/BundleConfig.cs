﻿using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;

namespace BlackMesa.Blog.Main.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            var cssTransformer = new CssTransformer();
            var jsTransformer = new JsTransformer();
            var cssMinifier = new CssMinify();
            var jsMinifier = new JsMinify();
            var nullOrderer = new NullOrderer();

            // Script Bundles

//            const string jQueryCdnPath = "http://code.jquery.com/jquery-1.9.1.min.js";

            bundles.Add(new ScriptBundle("~/bundles/global")
                .Include("~/Scripts/jquery-{version}.js", "~/Scripts/jquery-ui-1.10.2.custom.js", "~/Scripts/bootstrap.js", "~/Scripts/shCore.js", "~/Scripts/shAutoloader.js", "~/Scripts/shLegacy.js", "~/Scripts/jquery.taghandler.js", "~/Scripts/global.js", "~/Scripts/jquery.unobtrusive*", "~/Scripts/jquery.validate*", "~/Scripts/google-analytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin")
                .Include("~/Scripts/bootstrap-datetimepicker.js", "~/Scripts/jquery-autogrow-textarea.js", "~/Scripts/ace/ace.js", "~/Scripts/ace/theme-clouds.js", "~/Scripts/ace/mode-html.js"));
            
 
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            // Style Bundles

            var mainBundle = new Bundle("~/Content/global")
                .Include("~/Content/styles/bootstrap.less")
                .Include("~/Content/styles/jquery-ui-1.10.2.custom.less")
                .Include("~/Content/styles/shCoreDefault.less")
                .Include("~/Content/styles/shThemeDefault.less")
                .Include("~/Content/styles/jquery.taghandler.less");
                
            mainBundle.Transforms.Add(cssTransformer);
            mainBundle.Transforms.Add(cssMinifier);
            mainBundle.Orderer = nullOrderer;
            bundles.Add(mainBundle);

            var adminBundle = new Bundle("~/Content/admin")
                .Include("~/Content/styles/bootstrap-datetimepicker.less")
                .Include("~/Content/styles/bootstrap-tagmanager.less");
            adminBundle.Transforms.Add(cssTransformer);
            mainBundle.Transforms.Add(cssMinifier);
            adminBundle.Orderer = nullOrderer;
            bundles.Add(adminBundle);


//            BundleTable.EnableOptimizations = true;  // executing this line will force bundling and minification by overwriting whatever stands in web.config
//            #if DEBUG
//                BundleTable.EnableOptimizations = false;
//            #endif

        }
    }
}