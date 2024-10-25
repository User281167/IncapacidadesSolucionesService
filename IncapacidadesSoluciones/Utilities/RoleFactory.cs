namespace IncapacidadesSoluciones.Utilities
{
    public class RoleFactory
    {
        public static string GetRoleName(ROLE role)
        {
            switch (role)
            {
                case ROLE.AUXILIAR: return "AUXILIAR";
                case ROLE.RECEPCIONISTA: return "RECEPCIONISTA";
                case ROLE.ASESOR: return "ASESOR";
                case ROLE.LIDER: return "LIDER";
                case ROLE.COLABORADOR: return "COLABORADOR";
                case ROLE.GESTION_DOCUMENTAL: return "GESTION_DOCUMENTAL";
                case ROLE.CARTERA_JURIDICA: return "CARTERA_JURIDICA";
                case ROLE.CONTABILIDAD: return "CONTABILIDAD";
                default: return "";
            }
        }
    }
}
