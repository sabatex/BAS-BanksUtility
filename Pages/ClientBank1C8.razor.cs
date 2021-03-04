﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using sabatex.V1C8.BankHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Pages
{
    public partial class ClientBank1C8
    {
        class ResultTask
        {
            public string InputFile { get; set; }
            public string OutPutFile { get; set; }
            public string Result { get; set; }
        }


        private EBankType bankType = EBankType.iBankUA_TXT;
        private static Dictionary<EBankType, string> stringBankPresents = new Dictionary<EBankType, string>
        {
            {EBankType.iFobsUA_XML, "iFobs формат файла XML (ZIP)"},
            {EBankType.iBankUA_TXT, "iBank формат файла ТХТ (csv)"},
            {EBankType.PrivatUA,"ПриватБанк формат файла ТХТ (csv)"},
            {EBankType.OtpBankSK,"OTP Bank Словатчина формат  файла CSV"},
            {EBankType.PrimaBankSK,"PrimaBanka Словатчина формат  файла CSV"}
        };

        string accountNumber = "";
        string error = "";
        List<ResultTask> makeFiles = new List<ResultTask>();


        async Task HandleSelection(InputFileChangeEventArgs files)
        {
            byte[] buffer = new byte[512000];
            error = "";
            makeFiles.Clear();
            var mf = new List<string>();
            foreach (var f in files.GetMultipleFiles())
            {
                var resultTask = new ResultTask
                {
                    InputFile = f.Name,
                    OutPutFile = f.Name + ".xml",
                    Result = "Ok"
                };
                try
                {
                    using (MemoryStream memStream = new MemoryStream(512000))
                    {
                        await f.OpenReadStream().CopyToAsync(memStream);
                        string r = _1CClientBankExchange.ConvertTo1CFormat(bankType, memStream, accountNumber);
                        string fileName = "kb_to_1c" + f.Name + ".xml";
                        await JSRuntime.InvokeAsync<object>("sabatex.downloadFile", fileName,r);
                    }
                }
                catch (Exception e)
                {
                    resultTask.Result = e.Message;
                }
                makeFiles.Add(resultTask);
            }
        }
        void BankTypeChange(EBankType value)
        {
            bankType = value;
        }



    }
}
