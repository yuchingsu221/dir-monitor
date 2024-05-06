using CommonLibrary.Crypto;
using CommonLibrary.Util;
using DataLayer.RelationDB.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Models.Config;
using Domain.Models.RelationDB;
using Jobs.Models.AppRequest;
using Jobs.Services.Interfaces;
using Newtonsoft.Json;
using OfficeOpenXml;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebService.Models.AppRequest;

namespace Jobs.Jobs
{
    [DisallowConcurrentExecution]
    public class SampleJob : IJob
    {
        private readonly WebServiceSetting _Settings;

        public SampleJob(WebServiceSetting settings)
        {          
            _Settings = settings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await AddImgDoc();
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("ScanPushMessageJob Excute Exception", ex);
            }
        }

        private Task AddImgDoc()
        {
            // 1. 取得未上傳(List)
            List<GetImageData> unPushs = null; //_incomingDBAccess.GetImageIsNotUpload();
            var watch = new Stopwatch();

            // 2. 沒資料, 休息一下並結束
            if (unPushs == null || unPushs.Count == 0)
            {
                Thread.Sleep(5 * 1000); // 沒資料, 休息五秒
                return Task.CompletedTask;
            }

            List<BaseId> mainList = new List<BaseId>();
            mainList = unPushs.Select(a => new BaseId() { ID = a.ID, DocType = a.Type, DocImage = a.Photo }).ToList();

            // 3. 有資料, 逐筆更新資料表, 並迴圈發送(發API)
            //foreach (var unPushData in unPushs)
            //{
            //在本次批次執行後，將失敗的案件與錯誤訊息寄email給窗口
            List<AddImgDocDataModelError> addImgDocDataModel = new List<AddImgDocDataModelError>();
            try
            {
                #region 只要滿 設定張數 while list，串接 AddImgDoc，申貸資料(進件影像檔案)
                AddImgDocErrorRsModel addImgDocRsModel = new AddImgDocErrorRsModel();
                while (mainList.Count > 0)
                {
                    List<BaseId> supPassList = new List<BaseId>();
                    //當 mainList 小於 設定張數 就用剩下張數
                    int lastCount = _Settings.UploadImgLimit > mainList.Count ? mainList.Count : _Settings.UploadImgLimit;
                    supPassList.AddRange(mainList.GetRange(0, lastCount));
                    mainList.RemoveAll(doc => supPassList.Contains(doc));

                    AddImgDocRqModel addImgDocRqModel = new AddImgDocRqModel();
                    AddImgDocContentModel addImgContent = new AddImgDocContentModel();
                    List<AddImgDocInfoModel> addImgDocInfoModel = new List<AddImgDocInfoModel>();
                    addImgDocInfoModel = supPassList.Select(a => new AddImgDocInfoModel() { DocType = a.DocType, DocImage = a.DocImage }).ToList();

                    addImgContent.AppNo = unPushs.ToList().FirstOrDefault().CaseNum;
                    addImgContent.ImageInfo = addImgDocInfoModel;
                    addImgDocRqModel.Content = addImgContent;

                    watch.Start();
                    //addImgDocRsModel = _service.AddImgDoc(addImgDocRqModel);
                    watch.Stop();
                    LogUtility.LogInfo($"AddImgDoc time: {watch.ElapsedMilliseconds / 1000.0f}秒");
                    watch.Reset();

                    List<string> ids = supPassList.Select(x => x.ID).ToList();

                    if (200 == addImgDocRsModel.Resultcode)
                    {
                        foreach (var id in ids)
                        {
                            //_IncomingDBAccess.UpdateImageIsUpload(id, "1");
                        }
                    }
                    else
                    {
                        foreach (var id in ids)
                        {
                            //_IncomingDBAccess.UpdateImageIsUploadToFalse(id, "0", addImgDocRsModel.ResultMessage);
                        }
                        LogUtility.LogInfo($"AddImgDocRsModelResult: {JsonConvert.SerializeObject(addImgDocRsModel)}");

                        //foreach(var item in supPassList)
                        //{
                        AddImgDocDataModelError getUploadErrorExcel = new AddImgDocDataModelError(); //_incomingDBAccess.GetUploadErrorExcel(addImgContent.AppNo);
                        addImgDocDataModel.Add(new AddImgDocDataModelError
                        {
                            CaseNo = addImgContent.AppNo,
                            IncomingTime = getUploadErrorExcel.IncomingTime,
                            IDCard = getUploadErrorExcel.IDCard,
                            FullName = getUploadErrorExcel.FullName,
                            MobilePhone = getUploadErrorExcel.MobilePhone,
                            Email = getUploadErrorExcel.Email
                        });
                        //}                       
                    }
                }
                #endregion   

                if (addImgDocDataModel.Count > 0)
                {
                    #region 匯出Excel
                    ExcelPackage package = Export(addImgDocDataModel.Distinct().ToList());
                    package.Encryption.Password = "123";
                    //package.SaveAs("D:\\321.xlsx");

                    byte[] workbookArray = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //存檔至指定位置
                        package.SaveAs(ms);
                        workbookArray = ms.ToArray();
                    }
                    #endregion

                    #region 寄信給分行櫃員
                    string subject = "fail to transfer to";
                    string title = "FAIL NOTICE";
                    string content = $"The application is waiting for your calling the customer. Please find the detail as attachment file";

                    MailSendRqModel mailSendRq = new MailSendRqModel();
                    string[] receiverMails;
                    receiverMails = _Settings.MailSender.Split(",");
                    foreach (var receiverMail in receiverMails)
                    {
                        mailSendRq.Type = 0;
                        //mailSendRq.Receiver = _Settings.Email;
                        mailSendRq.Receiver = receiverMail.Trim();
                        //mailSendRq.MailSubject = emailModel.Subject;
                        //mailSendRq.MailContent = emailModel.Content;
                        mailSendRq.CustId = "User Group";
                        mailSendRq.MEFileName1 = String.Concat(DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd_ADDIMGDOC_ERROR_ISSUE"), ".xlsx");
                        mailSendRq.MEFileStream1 = workbookArray;
                        mailSendRq.METMnemonic = "上傳圖片失敗";

                        //LogUtility.LogInfo($"上傳圖片失敗，寄信給分行櫃員\n mailSendRq: {JsonConvert.SerializeObject(mailSendRq)}");

                        //_EmailService.MailSend(mailSendRq);
                    }
                    #endregion
                }
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError(@$"[批次]申請-上傳圖片失敗 :{ex.StackTrace}", ex);
            }
            finally
            {
                Thread.Sleep(1); // 發送結束, 休息1s
            }
            //}

            Thread.Sleep(1 * 1000); // 發送結束, 休息1s

            return Task.CompletedTask;
        }

        /// <summary>
        /// 產生 excel
        /// </summary>
        /// <param name="data">物件資料集</param>
        /// <returns></returns>
        public ExcelPackage Export(List<AddImgDocDataModelError> data)
        {
            //建立 excel 物件
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage();
            //ExcelPackage package = new ExcelPackage(new FileInfo("D:\\123.xlsx"));

            //加入 excel 工作表名為 `Report`
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Supplementary");

            sheet.Cells[1, 1].Value = "CaseNo";
            sheet.Cells[1, 2].Value = "IncomingTime";
            sheet.Cells[1, 3].Value = "IDCard";
            sheet.Cells[1, 4].Value = "FullName";
            sheet.Cells[1, 5].Value = "MobilePhone";
            sheet.Cells[1, 6].Value = "Email";

            //資料起始列位置
            int rowIdx = 2;
            data.ForEach(delegate (AddImgDocDataModelError item)
            {
                //每筆資料欄位起始位置
                int conlumnIndex = 1;
                foreach (var jtem in item.GetType().GetProperties())
                {
                    //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                    sheet.Cells[rowIdx, conlumnIndex].Value = Convert.ToString(jtem.GetValue(item, null));
                    conlumnIndex++;
                }
                rowIdx++;
            });

            return package;
        }
    }
}
