﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUploadServer
{
    public class UploadFileEntity
    {
        public int ReferenceId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        public string UploadFolder { get; set; }

        /// <summary>
        /// 文件二进制数据
        /// </summary>
        public byte[] FileData { get; set; }

        public bool GenerateSize { get; set; }
    }
}