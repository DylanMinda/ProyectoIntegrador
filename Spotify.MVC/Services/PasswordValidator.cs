namespace Spotify.MVC.Services
{
    using System.Text.RegularExpressions;

    public class PasswordValidator
    {
        public static class Requirements// Define la constante de requisitos para la contraseña
        {
            public const int MIN_LENGTH = 8;
            public const int MAX_LENGTH = 128;
        }

        public static ValidationResult ValidatePassword(string password)// Valida los requisitos de la contraseña
        {
            var result = new ValidationResult();// Crea una instancia de ValidationResult para almacenar los errores

            if (string.IsNullOrWhiteSpace(password))// Verifica si la contraseña está vacía o es nula
            {
                result.AddError("La contraseña es obligatoria.");
                return result;
            }

            // Validar longitud
            if (password.Length < Requirements.MIN_LENGTH)// Verifica si la contraseña cumple con la longitud mínima
            {
                result.AddError($"La contraseña debe tener al menos {Requirements.MIN_LENGTH} caracteres.");// Agrega un error si no cumple con la longitud mínima
            }

            if (password.Length > Requirements.MAX_LENGTH)// Verifica si la contraseña excede la longitud máxima
            {
                result.AddError($"La contraseña no puede tener más de {Requirements.MAX_LENGTH} caracteres.");// Agrega un error si excede la longitud máxima
            }

            // Validar caracteres requeridos
            if (!HasLowercase(password))// Verifica si la contraseña contiene al menos una letra minúscula
            {
                result.AddError("La contraseña debe contener al menos una letra minúscula.");
            }

            if (!HasUppercase(password))// Verifica si la contraseña contiene al menos una letra mayúscula
            {
                result.AddError("La contraseña debe contener al menos una letra mayúscula.");
            }

            if (!HasNumber(password))// Verifica si la contraseña contiene al menos un número
            {
                result.AddError("La contraseña debe contener al menos un número.");
            }

            if (!HasSpecialCharacter(password))// Verifica si la contraseña contiene al menos un carácter especial
            {
                result.AddError("La contraseña debe contener al menos un carácter especial (!@#$%^&*()_+-=[]{};':\"\\|,.<>/?).");
            }

            // Validar patrones débiles
            if (HasWeakPatterns(password))// Verifica si la contraseña contiene patrones débiles o comunes
            {
                result.AddError("La contraseña contiene patrones comunes que la hacen vulnerable. Por favor, elija una contraseña más segura.");
            }

            return result;
        }

        public static ValidationResult ValidateEmail(string email)// Valida el formato del correo electrónico
        {
            var result = new ValidationResult();// Crea una instancia de ValidationResult para almacenar los errores

            if (string.IsNullOrWhiteSpace(email))// Verifica si el correo electrónico está vacío o es nulo
            {
                result.AddError("El correo electrónico es obligatorio.");
                return result;
            }

            if (!IsValidEmailFormat(email))// Verifica si el correo electrónico tiene un formato válido
            {
                result.AddError("Por favor, ingrese un correo electrónico válido.");
            }

            return result;
        }

        public static ValidationResult ValidateName(string name)// Valida el nombre del usuario
        {
            var result = new ValidationResult();// Crea una instancia de ValidationResult para almacenar los errores

            if (string.IsNullOrWhiteSpace(name))// Verifica si el nombre está vacío o es nulo
            {
                result.AddError("El nombre es obligatorio.");
                return result;
            }

            if (name.Trim().Length < 2)// Verifica si el nombre tiene al menos 2 caracteres
            {
                result.AddError("El nombre debe tener al menos 2 caracteres.");
            }

            if (name.Length > 100)// Verifica si el nombre no excede los 100 caracteres
            {
                result.AddError("El nombre no puede tener más de 100 caracteres.");
            }

            return result;
        }

        public static ValidationResult ValidatePasswordConfirmation(string password, string confirmPassword)// Valida la confirmación de la contraseña
        {
            var result = new ValidationResult();// Crea una instancia de ValidationResult para almacenar los errores

            if (string.IsNullOrWhiteSpace(confirmPassword))// Verifica si la confirmación de la contraseña está vacía o es nula
            {
                result.AddError("La confirmación de contraseña es obligatoria.");
                return result;
            }

            if (password != confirmPassword)// Verifica si la contraseña y su confirmación coinciden
            {
                result.AddError("Las contraseñas no coinciden.");
            }

            return result;
        }

        public static List<string> GetPasswordRequirements()// Devuelve una lista de requisitos para la contraseña
        {
            return new List<string>// Devuelve una lista de requisitos para la contraseña
        {
            $"Al menos {Requirements.MIN_LENGTH} caracteres de longitud",
            "Al menos una letra minúscula (a-z)",
            "Al menos una letra mayúscula (A-Z)",
            "Al menos un número (0-9)",
            "Al menos un carácter especial (!@#$%^&*()_+-=[]{};':\"\\|,.<>/?)",
            "No debe contener patrones comunes como '123456', 'password', etc.",
            "No debe tener más de dos caracteres idénticos consecutivos"
        };
        }

        #region Private Methods

        private static bool HasLowercase(string password)// Verifica si la contraseña contiene al menos una letra minúscula
        {
            return Regex.IsMatch(password, @"[a-z]");// Utiliza una expresión regular para buscar letras minúsculas
        }

        private static bool HasUppercase(string password)// Verifica si la contraseña contiene al menos una letra mayúscula
        {
            return Regex.IsMatch(password, @"[A-Z]");// Utiliza una expresión regular para buscar letras mayúsculas
        }

        private static bool HasNumber(string password)// Verifica si la contraseña contiene al menos un número
        {
            return Regex.IsMatch(password, @"[0-9]");// Utiliza una expresión regular para buscar números
        }

        private static bool HasSpecialCharacter(string password)// Verifica si la contraseña contiene al menos un carácter especial
        {
            return Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");// Utiliza una expresión regular para buscar caracteres especiales
        }

        private static bool HasWeakPatterns(string password)// Verifica si la contraseña contiene patrones débiles o comunes
        {
            var commonPatterns = new[]// Define una lista de patrones comunes que hacen que la contraseña sea débil
            {
            @"(.)\1{2,}", // Tres o más caracteres repetidos consecutivos
            @"123|234|345|456|567|678|789|890|abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz",
            @"password|contraseña|12345|qwerty|admin|usuario|user|pass"
        };

            return commonPatterns.Any(pattern => Regex.IsMatch(password.ToLower(), pattern));// Utiliza una expresión regular para buscar patrones comunes en la contraseña
        }

        private static bool IsValidEmailFormat(string email)// Verifica si el correo electrónico tiene un formato válido
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);// Utiliza la clase MailAddress para validar el formato del correo electrónico
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    public class ValidationResult// Clase para almacenar los resultados de la validación
    {
        public bool IsValid => !Errors.Any();// Propiedad que indica si la validación fue exitosa (sin errores)
        public List<string> Errors { get; private set; } = new List<string>();// Lista para almacenar los errores de validación
        public string FirstError => Errors.FirstOrDefault() ?? "";// Propiedad que devuelve el primer error de validación, o una cadena vacía si no hay errores

        public void AddError(string error)// Método para agregar un error a la lista de errores
        {
            Errors.Add(error);// Agrega un error a la lista de errores
        }

        public void AddErrors(IEnumerable<string> errors)// Método para agregar múltiples errores a la lista de errores
        {
            Errors.AddRange(errors);// Agrega una colección de errores a la lista de errores
        }
    }
}
