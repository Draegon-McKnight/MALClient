﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace MALClient.Comm
{
    public abstract class Query
    {
        protected WebRequest Request;

        public async Task<string> GetRequestResponse()
        {
            string responseString = "";
            try
            {
                var response = await Request.GetResponseAsync();

                
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                    reader.Dispose();
                }
                return responseString;
            }
            catch (Exception e)
            {
                var msg = new MessageDialog(e.Message,"An error occured");
                await msg.ShowAsync();
            }
            return responseString;
        }

    }

    
}
