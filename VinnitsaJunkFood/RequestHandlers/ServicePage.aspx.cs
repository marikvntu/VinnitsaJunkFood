using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VinnitsaJunkFood.Logger;

namespace VinnitsaJunkFood
{
    public partial class ServicePage : System.Web.UI.Page
    {
        private FileLogger logger;

        protected void Page_Load(object sender, EventArgs e)
        {
            logger = new FileLogger();
        }

        protected void WriteToLog_Click(object sender, EventArgs e){
            try{
                logger.WriteLog(LogText.Text);
            }
            catch (Exception ex) {
                ErrorLabel.Text = ex.Message;
            }
        }
    }
}