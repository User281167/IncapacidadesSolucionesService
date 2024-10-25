namespace IncapacidadesSoluciones.Utilities.Role
{
    public class UserRoleFactory
    {
        public static string GetRoleName(USER_ROLE role)
        {
            switch (role)
            {
                case USER_ROLE.ASSISTANT: return "AUXILIAR";
                case USER_ROLE.RECEPTIONIST: return "RECEPCIONISTA";
                case USER_ROLE.ADVISER: return "ASESOR";
                case USER_ROLE.LEADER: return "LIDER";
                case USER_ROLE.COLLABORATOR: return "COLABORADOR";
                case USER_ROLE.DOCUMENTAL_MANAGEMENT: return "GESTION_DOCUMENTAL";
                case USER_ROLE.LEGAL_PORTFORT: return "CARTERA_JURIDICA";
                case USER_ROLE.ACCOUNTING: return "CONTABILIDAD";
                default: return "";
            }
        }
    }
}
