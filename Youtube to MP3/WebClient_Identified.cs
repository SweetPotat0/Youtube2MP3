using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Youtube_to_MP3
{
    class WebClient_Identified : WebClient
    {
        public int id = 0;
        public string url,
            name;
        public Double length;
        public long bytesToRe=-1;
        public int tries = 0;
        public byte lengthError = 0;
        public bool isBurned { get { return tries > 5; } }
        public WebClient_Identified() : base() {
            this.id = 0;
        }
        public WebClient_Identified(int id) : base() {
            this.id = id;
        }
        public WebClient_Identified(int id,string url,string name): base()
        {
            this.id = id;
            this.url = url;
            this.name = name;
        }
        public void setBytesToRecive(long bytesToRecive) {
            this.bytesToRe = bytesToRecive;
        }
    }
}
