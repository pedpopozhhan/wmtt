using System;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

public class CASVendorAddress : ICASAudit
{
    public int VENDOR_ADDRESS_ID { get; set; }//	NUMBER(10,0)
    public int ADDRESS_SEQ_NUM { get; set; }// NUMBER(5,0)
    public DateTime EFFDT { get; set; }//  DATE
    public string ADDRESS1 { get; set; }//	VARCHAR2(55 BYTE)
    public string ADDRESS2 { get; set; }//	VARCHAR2(55 BYTE)
    public string ADDRESS3 { get; set; }//	VARCHAR2(55 BYTE)
    public string CITY { get; set; }//	VARCHAR2(30 BYTE)
    public string STATE { get; set; }//	VARCHAR2(6 BYTE)
    public string POSTAL { get; set; }//	VARCHAR2(12 BYTE)
    public string COUNTRY { get; set; }//	VARCHAR2(3 BYTE)
    public string PHONE_NUMBER { get; set; }//	VARCHAR2(24 BYTE)
    public string PHONE_EXTENSION { get; set; }//	VARCHAR2(6 BYTE)
    public string COUNTRY_NUMBER { get; set; }//	VARCHAR2(3 BYTE)
    public string FAX_NUM { get; set; }//	VARCHAR2(24 BYTE)
    public int VENDOR_PK_ID { get; set; }//	NUMBER(10,0)
    public string ADDRESS_STATUS { get; set; }//	VARCHAR2(1 BYTE)
    public string ADDRESS4 { get; set; }//	VARCHAR2(55 BYTE)
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string UpdateUserId { get; set; }
}