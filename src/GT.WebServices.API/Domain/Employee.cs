using System;

namespace GT.WebServices.API.Domain
{
    public partial class Employee
    {
        public Guid EmployeeId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string BadgeCode { get; set; }
        public string KeyPadId { get; set; }
        public string PinCode { get; set; }
        public string Language { get; set; }
        public string Roles { get; set; }
        public string VerifyBy { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Photo { get; set; }

        public string FingerTemplates { get; set; }
        public string FaceTemplates { get; set; }
        public bool IsDeleted { get; set; }
        public string ObjectHash { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }


}
