using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security;

namespace SharePointFileTransfer_Library
{
    public static class SharePointFileTransfer
    {
        #region Propertys
        public static string uname { get; private set; }
        public static string pswstring { get; private set; }
        public static string path { get; private set; }

        private static ClientContext cc;
        private static string exPath = "./SPFT_Exceptions.txt";

        #endregion

        #region Public Methods
        public static void Connect ()
        {
            if (cc == null)
            {
                Init();
                cc = getCC();
            }
        }

        public static void UploadFiles(string DocName, string localPath)
        {
            checkCC();
                
            List oList = cc.Web.Lists.GetByTitle("AppContent");
            FileCreationInformation ft = new FileCreationInformation();

            try { ft.Content = System.IO.File.ReadAllBytes(localPath); }
            catch (Exception e) {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString() 
                    + "> Couldn't read the local File; ErrorMsg: " + e.Message;

                System.IO.File.WriteAllText(exPath, textToWrite);
            }

            ft.Overwrite = true;
            ft.Url = DocName;

            try
            {
                File destfile = oList.RootFolder.Files.Add(ft);
            }
            catch (Exception e)
            {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString()
                    + "> File couldn't be added; ErrorMsg: " + e.Message;

                System.IO.File.WriteAllText(exPath, textToWrite);
            }
            cc.ExecuteQuery();
        }

        public static void changeCredentials (string username, string decodedPsw)
        {
            ConfigurationManager.AppSettings.Set("user", username);
            ConfigurationManager.AppSettings.Set("psw", Crypter.Encrypt(decodedPsw));

            Init();
            try
            {
                checkCC();
                Disconnect();
                Connect();
            }
            catch(Exception e)
            {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString()
                    + "> Error while changing the Credentials; ErrorMsg: " + e.Message;

                System.IO.File.WriteAllText(exPath, textToWrite);
            }
        }

        public static void changePath (string path)
        {
            ConfigurationManager.AppSettings.Set("path", path);

            Init();
            try
            {
                checkCC();
                Disconnect();
                Connect();
            }
            catch (Exception e)
            {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString()
                    + "> Error while changing path; ErrorMsg: " + e.Message;

                System.IO.File.WriteAllText(exPath, textToWrite);
            }
        }

        public static void Disconnect()
        {
            if (cc != null)
                cc.Dispose();

            cc = null;
        }

        public static List getListItems()
        {
            checkCC();
            
            List oList = cc.Web.Lists.GetByTitle("AppContent");

            CamlQuery camlQuery = new CamlQuery();
            ListItemCollection collListItem = oList.GetItems(camlQuery);

            cc.Load(
                collListItem,
                items => items.Take(5).Include(
                item => item["Title"]));

            cc.ExecuteQuery();

            return oList;
        }
        #endregion

        #region Private Methods
        private static void Init()
        {
            try
            {
                uname = ConfigurationManager.AppSettings.Get("user");
                pswstring = ConfigurationManager.AppSettings.Get("psw");
                path = ConfigurationManager.AppSettings.Get("path");
            }
            catch (Exception e)
            {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString()
                    + "> Problem in Class: SharePointFileTransfer, Method: Init. Error Msg:  " + e.Message;

                System.IO.File.WriteAllText(exPath, textToWrite);
            }
        }

        private static ClientContext getCC()
        {
            ClientContext cc = new ClientContext(path);
            SecureString psw = new SecureString();
            Crypter.Decrypt(pswstring).ToCharArray().ToList().ForEach(p => psw.AppendChar(p));

            NetworkCredential nc = new NetworkCredential(uname, psw);
            nc.Domain = "htl-vil";
            cc.Credentials = nc;

            return cc;
        }
        private static void checkCC()
        {
            if (cc == null)
            {
                string oldText = "";
                if (System.IO.File.Exists(exPath))
                    oldText = System.IO.File.ReadAllText(exPath);

                string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString()
                    + "> CC not connected.";

                System.IO.File.WriteAllText(exPath, textToWrite);
            }
        }
        #endregion
    }
}
