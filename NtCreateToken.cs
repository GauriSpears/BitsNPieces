//нужно установить NuGet пакет NtApiDotNet
//копируем токен процесса lsass.exe, чтобы получить привилегию SeCreateTokenPrivilege
int pid = System.Diagnostics.Process.GetProcessesByName("lsass")[0].Id;
NtProcess ntProcess = NtProcess.Open(pid, ProcessAccessRights.QueryLimitedInformation); //для XP использовать ProcessAccessRights.QueryInformation
NtToken pTokenOrig= NtToken.OpenProcessToken(ntProcess, false, TokenAccessRights.MaximumAllowed);
//токен от lsass.exe
NtToken ntTokenDub = pTokenOrig.DuplicateToken(TokenType.Impersonation,SecurityImpersonationLevel.Impersonation, (TokenAccessRights)TOKEN_ALL_ACCESS | TokenAccessRights.GenericAll | TokenAccessRights.MaximumAllowed | TokenAccessRights.Duplicate);
//токен конкретного пользователя
NtToken ntTokenNewDub = null;
//получаем токен пользователя
using (ThreadImpersonationContext tic = ntTokenDub.Impersonate())
{
   // try
   // {
        NtToken ntTokenNew = NtToken.Create(sid);
        ntTokenNewDub = ntTokenNew.DuplicateToken(TokenType.Impersonation, SecurityImpersonationLevel.Impersonation, (TokenAccessRights)TOKEN_ALL_ACCESS | TokenAccessRights.GenericAll | TokenAccessRights.MaximumAllowed | TokenAccessRights.Duplicate);
   // }
   // catch (Exception e)
   // {
   //     errcode = e.HResult;
   // }
   // finally
   // {
        tic.Revert();
   // }
}
//возможно, тут нужен LoadUserProfile(ntTokenDub.Handle.DangerousGetHandle(), ref profileInfo);, но я не заметил, чтобы что-то менялось
//запускаем код под пользователем
bool res = false;
if (ntTokenNewDub != null)
{
    using (ThreadImpersonationContext tic = ntTokenNewDub.Impersonate())
    {
        //try
        //{
            res = true;//код писать сюда
        //}
        //catch (Exception e)
        //{
        //    errcode = e.HResult;
        //}
        //finally
        //{
            tic.Revert();
        //}
    }
}
