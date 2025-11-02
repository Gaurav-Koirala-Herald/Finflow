public class JwtSettings
{
  public string jwtSecret {get;set;}
  public string issuer {get;set;}
  public string audience {get;set;}
  public int accessTokeExpirationMinutes {get;set;}
  public int refreshTokenExpirationMinutes{get;set;}
}
