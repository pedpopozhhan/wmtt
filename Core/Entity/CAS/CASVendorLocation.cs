using System;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

public class CASVendorLocation : ICASAudit
{
    public int VENDOR_LOCATION_ID { get; set; }//	NUMBER(10,0)
    public string LOCATION_NUM { get; set; }//	VARCHAR2(10 BYTE)
    public int VENDOR_PK_ID { get; set; }//	NUMBER(10,0)
    public DateTime EFFDT { get; set; }//	DATE
    public string DESCRIPTION { get; set; }//	VARCHAR2(30 BYTE)
    public string CURRENCY_CD { get; set; }//	VARCHAR2(3 BYTE)
    public string PYMNT_TERMS_CD { get; set; }//	VARCHAR2(5 BYTE)
    public string PYMNT_HOLD { get; set; }//	VARCHAR2(1 BYTE)
    public string PYMNT_HANDLING_CD { get; set; }//	VARCHAR2(2 BYTE)
    public string PYMNT_METHOD { get; set; }//	VARCHAR2(3 BYTE)
    public string REMIT_VENDOR_SETID { get; set; }//	VARCHAR2(5 BYTE)
    public string REMIT_VENDOR { get; set; }//	VARCHAR2(10 BYTE)
    public string REMIT_ADDR_SEQ_NUM { get; set; }//	NUMBER(5,0)
    public string REMIT_LOCATION { get; set; }//	VARCHAR2(10 BYTE)
    public string PRICE_VENDOR_SETID { get; set; }//	VARCHAR2(5 BYTE)
    public string PRICE_VENDOR { get; set; }//	VARCHAR2(10 BYTE)
    public string PRICE_LOCATION { get; set; }//	VARCHAR2(10 BYTE)
    public string RETURN_VENDOR { get; set; }//	VARCHAR2(10 BYTE)
    public string BUYER_ID { get; set; }//	VARCHAR2(10 BYTE)
    public string LOCATION_STATUS { get; set; }//	VARCHAR2(1 BYTE)
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string UpdateUserId { get; set; }
}