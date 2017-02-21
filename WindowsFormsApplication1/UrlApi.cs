using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    internal class UrlApi
    {
        internal string Fbaseurl;
        internal string Finterface;
        internal string Fmethod;
        internal string Fversion;
        internal string Fkey;
        internal string Fsteamid;

        public string getUrl()
        {
            string vlink = "https://" + Fbaseurl + "/" + Finterface + "/" + Fmethod + "/" + Fversion + "/";
            if( (Fkey == null) && (Fsteamid == null) ){
                return vlink;
            } 
            else {
                vlink = vlink + "?";
                bool fl = false;
                if (Fkey != null) { vlink = vlink + "key=" + Fkey; fl = true; }
                if (Fsteamid != null) {
                    if (fl) { vlink = vlink + "&"; }
                    vlink = vlink + "steamids=" + Fsteamid;
                    fl = true;
                }
                return vlink;
            }
            
        }
    }
}
