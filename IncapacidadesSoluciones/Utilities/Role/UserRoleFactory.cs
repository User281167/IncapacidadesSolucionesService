namespace IncapacidadesSoluciones.Utilities.Role
{
    public class UserRoleFactory
    {
        public readonly static Dictionary<USER_ROLE, string> roles = new Dictionary<USER_ROLE, string>()
            {
                {USER_ROLE.ASSISTANT , "AUXILIAR"},
                {USER_ROLE.RECEPTIONIST, "RECEPCIONISTA"},
                {USER_ROLE.ADVISER, "ASESOR"},
                {USER_ROLE.LEADER, "LIDER"},
                {USER_ROLE.COLLABORATOR, "COLABORADOR"},
                {USER_ROLE.DOCUMENTAL_MANAGEMENT, "GESTION_DOCUMENTAL"},
                {USER_ROLE.LEGAL_PORTFORT, "CARTERA_JURIDICA"},
                {USER_ROLE.ACCOUNTING , "CONTABILIDAD"},
                {USER_ROLE.NOT_FOUND , "NOT_FOUND"},
            };

        public static string GetRoleName(USER_ROLE role)
        {
            return roles.TryGetValue(role, out var value) ? value : "";
        }

        public static USER_ROLE GetRole(string roleName)
        {
            var item = roles.SingleOrDefault(
                r => r.Value == roleName.ToUpper(), 
                new KeyValuePair<USER_ROLE, string>(USER_ROLE.NOT_FOUND, "")
                );

            return item.Key;
        }
    }
}
