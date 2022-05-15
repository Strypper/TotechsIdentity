namespace TotechsIdentity.Constants
{
    public static class SwaggerConstants
    {

        public const string Url                       = "/swagger/v1/swagger.json";
        public const string Title                     = "TotechsIdentity";
        public const string Scheme                    = "Bearer";
        public const string Description               = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"";
        public const string OpenAPIVersion            = "v1";
        public const string SwaggerEndPointName       = "TotechsIdentity v1";
        public const string SecurityDefinitionName    = "Bearer";
        public const string OpenApiSecuritySchemeName = "Authorization";

    }

    public static class JwtTokenConstants
    {
        public const string GenericIdentityType = "TokenAuth";
    }

    public static class TotechsConstants
    {
        public const string ServiceName = "Totechs";
    }

    public static class EmailConstants
    {
        public const string EmailConfirmation   = "Email Confirmation";
        public const string SuccessHtmlTemplate = "<html><body><h1>Email confirmed successfully</h1></body></html>";
        public const string ContentType         = "text/html";
    }
}
