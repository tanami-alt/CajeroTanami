# GUÍA DE SUSTENTACIÓN - CAJERO AUTOMÁTICO

## 1. INTRODUCCIÓN AL PROYECTO

### ¿Qué es este programa?
- **Propósito**: Simula un cajero automático básico
- **Funciones**: Depositar, retirar, consultar saldo, ver movimientos, cambiar PIN
- **Tecnología**: C# con .NET 8 (aplicación de consola)

### ¿Por qué C#?
- **Lenguaje moderno**: Fácil de aprender, potente
- **.NET**: Plataforma robusta de Microsoft
- **Consola**: Interfaz simple, perfecta para aprender conceptos básicos

---

## 2. ESTRUCTURA DEL PROYECTO

### Archivos principales:
```
CajeroTanami/
├── Program.cs          ← Punto de entrada (Main)
├── Cajero.cs          ← Lógica de negocio
├── Usuario.cs         ← Modelo de datos usuario
├── Movimiento.cs      ← Modelo de datos movimiento
├── ArchivoHelper.cs   ← Manejo de archivos CSV
└── CajeroAutomatico.csproj ← Configuración del proyecto
```

### ¿Por qué separar en archivos?
- **Organización**: Cada archivo tiene una responsabilidad específica
- **Mantenimiento**: Más fácil encontrar y modificar código
- **Reutilización**: Las clases se pueden usar en otros proyectos

---

## 3. EXPLICACIÓN PASO A PASO

### 3.1 Program.cs - El "Cerebro" Principal

```csharp
static void Main()
{
    var cajero = new Cajero();  // Crear instancia del cajero
    cajero.AsegurarUsuarioDemo(); // Crear usuario de prueba
```

**¿Qué hace Main()?**
- Es el punto de entrada del programa
- Se ejecuta primero cuando inicias la aplicación
- Coordina todo el flujo del programa

**Conceptos clave:**
- `var`: Tipo de variable automático (C# infiere el tipo)
- `new Cajero()`: Crear un objeto de la clase Cajero
- `static`: Método que pertenece a la clase, no a una instancia

### 3.2 Flujo de Login

```csharp
System.Console.Write("Usuario (Id o Nombre): ");
var user = System.Console.ReadLine() ?? string.Empty;
System.Console.Write("PIN: ");
var pin = LeerPin();
```

**¿Qué hace?**
- Pide credenciales al usuario
- `Console.ReadLine()`: Lee texto del teclado
- `?? string.Empty`: Si es null, usa cadena vacía
- `LeerPin()`: Función personalizada que oculta el PIN con asteriscos

### 3.3 Menú Principal (Bucle while)

```csharp
while (true)  // Bucle infinito
{
    // Mostrar opciones
    switch (opcion)  // Evaluar la opción elegida
    {
        case "1": RealizarDeposito(cajero); break;
        case "2": RealizarRetiro(cajero); break;
        // ... más casos
    }
}
```

**¿Por qué while(true)?**
- El menú debe repetirse hasta que el usuario elija "Salir"
- `break` sale del switch, `return` sale del método Main

---

## 4. CLASES Y OBJETOS

### 4.1 Clase Usuario (Modelo de Datos)

```csharp
public class Usuario
{
    public string Id { get; set; }      // Identificador único
    public string Nombre { get; set; }  // Nombre del usuario
    public string Pin { get; set; }     // Contraseña
    public decimal Saldo { get; set; }  // Dinero disponible
}
```

**Conceptos:**
- `class`: Define un tipo de objeto
- `public`: Accesible desde otros archivos
- `{ get; set; }`: Propiedades automáticas (C# genera el código)

### 4.2 Clase Movimiento (Historial)

```csharp
internal class Movimiento
{
    public DateTime Fecha { get; set; }           // Cuándo ocurrió
    public string Tipo { get; set; }              // Qué tipo (Deposito, Retiro, etc.)
    public decimal Monto { get; set; }            // Cuánto dinero
    public decimal SaldoResultante { get; set; }  // Saldo después de la operación
}
```

**¿Por qué separar Usuario y Movimiento?**
- **Usuario**: Datos personales y saldo actual
- **Movimiento**: Historial de transacciones
- **Separación de responsabilidades**: Cada clase tiene un propósito específico

---

## 5. LÓGICA DE NEGOCIO (Cajero.cs)

### 5.1 Inicio de Sesión

```csharp
public bool IniciarSesion(string identificador, string pin)
{
    var usuario = _usuarios.FirstOrDefault(u =>
        (string.Equals(u.Id, identificador, StringComparison.OrdinalIgnoreCase)
         || string.Equals(u.Nombre, identificador, StringComparison.OrdinalIgnoreCase))
        && u.Pin == pin);
    if (usuario == null) return false;
    _usuarioActual = usuario;
    return true;
}
```

**¿Qué hace?**
- Busca un usuario que coincida con ID o nombre
- Verifica que el PIN sea correcto
- Si encuentra coincidencia, lo guarda como usuario actual
- `FirstOrDefault`: Encuentra el primer elemento que cumple la condición

### 5.2 Depositar Dinero

```csharp
public bool Depositar(decimal monto)
{
    if (_usuarioActual == null) return false;  // No hay sesión
    if (monto <= 0) return false;              // Monto inválido

    _usuarioActual.Saldo += monto;             // Sumar al saldo
    
    // Crear registro del movimiento
    var movimiento = new Movimiento
    {
        Fecha = DateTime.Now,
        Tipo = "Deposito",
        Monto = monto,
        SaldoResultante = _usuarioActual.Saldo
    };
    
    ArchivoHelper.AgregarMovimiento(_usuarioActual.Id, movimiento);
    ArchivoHelper.GuardarUsuarios(_usuarios);
    return true;
}
```

**Validaciones importantes:**
- Verificar que hay sesión activa
- Verificar que el monto sea positivo
- Actualizar saldo en memoria
- Registrar el movimiento
- Guardar cambios en archivo

### 5.3 Retirar Dinero

```csharp
public bool Retirar(decimal monto)
{
    if (_usuarioActual == null) return false;
    if (monto <= 0) return false;
    if (_usuarioActual.Saldo < monto) return false;  // Saldo insuficiente

    _usuarioActual.Saldo -= monto;  // Restar del saldo
    // ... resto igual que depositar
}
```

**Diferencia clave con depositar:**
- Verifica que hay suficiente saldo antes de retirar

---

## 6. PERSISTENCIA DE DATOS (ArchivoHelper.cs)

### 6.1 ¿Por qué CSV?
- **Simple**: Fácil de leer y escribir
- **Portable**: Se puede abrir en Excel
- **Sin base de datos**: No requiere instalación adicional

### 6.2 Estructura de archivos:

**usuarios.csv:**
```
Id,Nombre,Pin,Saldo
u1,Demo,1234,100.50
u2,Juan,5678,250.00
```

**movimientos.csv:**
```
UsuarioId,Fecha,Tipo,Monto,SaldoResultante
u1,2024-01-15T10:30:00.0000000,Deposito,100.00,100.00
u1,2024-01-15T10:35:00.0000000,Retiro,50.00,50.00
```

### 6.3 Cargar Usuarios

```csharp
public static List<Usuario> CargarUsuarios()
{
    var lista = new List<Usuario>();
    if (!File.Exists(UsuariosCsv)) return lista;  // Si no existe, lista vacía

    using var reader = new StreamReader(UsuariosCsv);
    string? header = reader.ReadLine();  // Saltar cabecera
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) continue;
        var cols = ParseCsvLine(line);  // Dividir por comas
        // ... crear objeto Usuario
    }
    return lista;
}
```

**Conceptos importantes:**
- `using`: Asegura que el archivo se cierre automáticamente
- `List<Usuario>`: Lista de objetos Usuario
- `ParseCsvLine`: Función personalizada para manejar comas en los datos

---

## 7. CONCEPTOS DE PROGRAMACIÓN APLICADOS

### 7.1 Programación Orientada a Objetos (POO)
- **Clases**: Plantillas para crear objetos (Usuario, Movimiento, Cajero)
- **Objetos**: Instancias de las clases (un usuario específico)
- **Encapsulación**: Los datos están dentro de las clases
- **Métodos**: Funciones que pertenecen a las clases

### 7.2 Manejo de Errores
```csharp
if (decimal.TryParse(input, out var monto))  // Intenta convertir
{
    // Éxito: usar monto
}
else
{
    // Error: mostrar mensaje
}
```

### 7.3 Validaciones
- **Entrada de usuario**: Verificar que los datos sean válidos
- **Estado del sistema**: Verificar que hay sesión activa
- **Lógica de negocio**: Verificar saldo suficiente para retiros

---

## 8. FLUJO COMPLETO DEL PROGRAMA

### 8.1 Flujo Detallado Paso a Paso

```
1. INICIO DEL PROGRAMA
   ↓
2. CREAR CAJERO
   - new Cajero() → Cargar usuarios desde CSV
   - AsegurarUsuarioDemo() → Crear usuario demo si no hay usuarios
   ↓
3. PANTALLA DE LOGIN
   - Mostrar "=== Cajero Automático ==="
   - Pedir Usuario (Id o Nombre)
   - Pedir PIN (oculto con asteriscos)
   ↓
4. VALIDAR CREDENCIALES
   - Buscar en lista de usuarios
   - Comparar ID/Nombre (sin importar mayúsculas)
   - Comparar PIN exacto
   - Si no coincide → Mostrar error y terminar
   - Si coincide → Guardar como usuario actual
   ↓
5. MENÚ PRINCIPAL (BUCLE INFINITO)
   - Mostrar opciones 1-6
   - Leer selección del usuario
   - Ejecutar función correspondiente
   - Volver al menú (excepto opción 6)
   ↓
6. OPERACIONES (ejemplo: Depositar)
   - Validar que hay sesión activa
   - Pedir monto al usuario
   - Validar que monto > 0
   - Actualizar saldo en memoria
   - Crear objeto Movimiento
   - Guardar movimiento en CSV
   - Guardar usuarios actualizados en CSV
   - Mostrar confirmación
   ↓
7. SALIR
   - Mostrar "¡Hasta luego!"
   - Terminar programa
```

### 8.2 Flujo de Datos en Memoria

```
Memoria RAM:
┌─────────────────┐
│   List<Usuario> │ ← Cargado al inicio desde usuarios.csv
│   _usuarios     │
└─────────────────┘
         │
         ▼
┌─────────────────┐
│   Usuario       │ ← Usuario actualmente logueado
│   _usuarioActual│
└─────────────────┘
         │
         ▼
┌─────────────────┐
│   Operaciones   │ ← Depósitos, retiros, etc.
│   (modifican    │
│    el saldo)    │
└─────────────────┘
         │
         ▼
┌─────────────────┐
│   Archivos CSV  │ ← Cambios se guardan aquí
│   (disco duro)  │
└─────────────────┘
```

---

## 9. ARCHIVOS CSV - ¿QUÉ SON Y CÓMO FUNCIONAN?

### 9.1 ¿Qué es un archivo CSV?

**CSV = Comma Separated Values (Valores Separados por Comas)**

- **Archivo de texto plano**: Se puede abrir con cualquier editor de texto
- **Formato estándar**: Usado en Excel, Google Sheets, bases de datos
- **Estructura simple**: Cada línea es un registro, cada coma separa un campo

### 9.2 Ejemplo Visual de CSV

```
usuarios.csv:
Id,Nombre,Pin,Saldo
u1,Demo,1234,100.50
u2,Juan,5678,250.00
u3,Maria,9999,75.25
```

**¿Cómo se ve en un editor de texto?**
```
Id,Nombre,Pin,Saldo
u1,Demo,1234,100.50
u2,Juan,5678,250.00
u3,Maria,9999,75.25
```

**¿Cómo se ve en Excel?**
```
| Id | Nombre | Pin  | Saldo  |
|----|--------|------|--------|
| u1 | Demo   | 1234 | 100.50 |
| u2 | Juan   | 5678 | 250.00 |
| u3 | Maria  | 9999 | 75.25  |
```

### 9.3 Ventajas de usar CSV

✅ **Simplicidad**: Fácil de leer y escribir
✅ **Portabilidad**: Funciona en cualquier sistema operativo
✅ **Compatibilidad**: Se abre en Excel, Google Sheets, etc.
✅ **Sin dependencias**: No necesita base de datos
✅ **Debugging**: Puedes ver los datos directamente

### 9.4 Desventajas de CSV

❌ **Sin validaciones**: No hay restricciones de datos
❌ **Sin relaciones**: No puede conectar tablas fácilmente
❌ **Sin concurrencia**: Múltiples usuarios pueden corromper el archivo
❌ **Sin transacciones**: No hay rollback si algo falla

### 9.5 Cómo funciona el parser CSV en nuestro código

```csharp
private static List<string> ParseCsvLine(string line)
{
    var res = new List<string>();
    bool quoted = false;  // ¿Estamos dentro de comillas?
    var cur = new System.Text.StringBuilder();
    
    foreach (var c in line)
    {
        if (c == '"') 
        { 
            quoted = !quoted;  // Cambiar estado de comillas
            continue; 
        }
        if (c == ',' && !quoted)  // Coma fuera de comillas = separador
        {
            res.Add(cur.ToString());
            cur.Clear();
        }
        else
        {
            cur.Append(c);  // Agregar carácter al campo actual
        }
    }
    res.Add(cur.ToString());  // Agregar último campo
    return res;
}
```

**Ejemplo paso a paso:**
```
Entrada: "Juan,Pérez,1234,100.50"
Proceso:
- J-u-a-n → Campo 1: "Juan"
- , → Separador, empezar campo 2
- P-é-r-e-z → Campo 2: "Pérez"  
- , → Separador, empezar campo 3
- 1-2-3-4 → Campo 3: "1234"
- , → Separador, empezar campo 4
- 1-0-0-.-5-0 → Campo 4: "100.50"
Resultado: ["Juan", "Pérez", "1234", "100.50"]
```

---

## 10. CARPETAS BIN Y OBJ - ¿QUÉ SON?

### 10.1 Estructura de carpetas del proyecto

```
CajeroTanami/
├── Program.cs                    ← Tu código fuente
├── Cajero.cs
├── Usuario.cs
├── Movimiento.cs
├── ArchivoHelper.cs
├── CajeroAutomatico.csproj      ← Configuración del proyecto
├── bin/                         ← Archivos compilados (ejecutables)
│   └── Debug/
│       └── net8.0/
│           ├── CajeroAutomatico.exe    ← Programa ejecutable
│           ├── CajeroAutomatico.dll    ← Biblioteca compilada
│           ├── usuarios.csv            ← Datos de usuarios
│           └── movimientos.csv         ← Historial (se crea al usar)
└── obj/                         ← Archivos temporales de compilación
    └── Debug/
        └── net8.0/
            ├── CajeroAutomatico.dll    ← Versión temporal
            ├── archivos de caché
            └── archivos de compilación
```

### 10.2 Carpeta BIN (Binary - Binarios)

**¿Qué contiene?**
- **Archivos ejecutables**: El programa que realmente se ejecuta
- **Dependencias**: Librerías necesarias para que funcione
- **Datos**: Archivos CSV con la información del programa

**¿Por qué existe?**
- **Separación**: El código fuente está separado del programa final
- **Distribución**: Solo necesitas la carpeta bin para ejecutar el programa
- **Compilación**: Cuando compilas, .NET genera estos archivos

**Archivos importantes en bin:**
```
CajeroAutomatico.exe     ← El programa principal
CajeroAutomatico.dll     ← Librería con tu código compilado
usuarios.csv             ← Datos de usuarios (se crea aquí)
movimientos.csv          ← Historial de movimientos (se crea aquí)
```

### 10.3 Carpeta OBJ (Object - Objetos)

**¿Qué contiene?**
- **Archivos temporales**: Código compilado intermedio
- **Caché de compilación**: Para acelerar compilaciones futuras
- **Archivos de depuración**: Información para debuggear

**¿Por qué existe?**
- **Optimización**: .NET usa estos archivos para compilar más rápido
- **Debugging**: Contiene información para encontrar errores
- **Compilación incremental**: Solo recompila lo que cambió

**¿Se puede borrar?**
- ✅ **Sí**: Se regenera automáticamente al compilar
- ✅ **Recomendado**: Si tienes problemas de compilación, borra obj y bin
- ✅ **Limpieza**: `dotnet clean` borra estas carpetas

### 10.4 Proceso de compilación

```
1. CÓDIGO FUENTE (.cs)
   ↓
2. COMPILADOR C# (csc.exe)
   ↓
3. CARPETA OBJ (archivos temporales)
   ↓
4. ENLACES Y OPTIMIZACIONES
   ↓
5. CARPETA BIN (programa final)
```

**Comandos útiles:**
```bash
dotnet build          # Compilar el proyecto
dotnet run            # Compilar y ejecutar
dotnet clean          # Limpiar carpetas bin y obj
dotnet publish        # Crear versión para distribución
```

### 10.5 ¿Por qué los CSV están en BIN?

**Razón técnica:**
- El programa se ejecuta desde la carpeta bin
- Los archivos CSV se crean en el "directorio de trabajo actual"
- Por eso aparecen en `bin/Debug/net8.0/`

**¿Es esto correcto?**
- ✅ **Para desarrollo**: Está bien para aprender
- ⚠️ **Para producción**: Los datos deberían estar en una carpeta separada
- 💡 **Mejora**: Podrías usar rutas absolutas o configuración

---

## 11. FLUJO COMPLETO DEL PROGRAMA (ACTUALIZADO)

1. **Inicio**: Crear cajero, asegurar usuario demo
2. **Login**: Pedir credenciales, verificar en lista de usuarios
3. **Menú**: Mostrar opciones, procesar selección
4. **Operaciones**: Validar, ejecutar, registrar movimiento
5. **Persistencia**: Guardar cambios en archivos CSV
6. **Repetir**: Volver al menú hasta salir

---

## 12. PUNTOS FUERTES DEL CÓDIGO

### ✅ Lo que está bien:
- **Separación clara**: Cada clase tiene una responsabilidad
- **Validaciones**: Verifica datos antes de procesar
- **Interfaz amigable**: Menú claro con colores
- **Persistencia**: Los datos se guardan entre ejecuciones
- **Código limpio**: Fácil de leer y entender

### ⚠️ Áreas de mejora:
- **Seguridad**: PIN en texto plano (debería estar encriptado)
- **Concurrencia**: No maneja múltiples usuarios simultáneos
- **Validaciones**: Podría ser más estricto con formatos de entrada

---

## 13. CÓMO DEMOSTRAR EL PROGRAMA

### Pasos para la demostración:
1. **Ejecutar**: `dotnet run`
2. **Login**: Usuario "u1" o "Demo", PIN "1234"
3. **Depositar**: Mostrar validación de montos
4. **Consultar saldo**: Ver el saldo actualizado
5. **Ver movimientos**: Mostrar historial
6. **Retirar**: Demostrar validación de saldo insuficiente
7. **Cambiar PIN**: Mostrar actualización de credenciales

### Puntos clave a mencionar:
- "El programa valida cada entrada del usuario"
- "Los datos se guardan automáticamente en archivos"
- "Cada operación queda registrada en el historial"
- "El código está organizado en clases para facilitar el mantenimiento"

---

## 14. PREGUNTAS FRECUENTES Y RESPUESTAS

**P: ¿Por qué usar C# en lugar de otro lenguaje?**
R: C# es moderno, tiene excelente soporte en .NET, y es fácil de aprender. Además, tiene características de seguridad y manejo de memoria automático.

**P: ¿Por qué no usar una base de datos?**
R: Para un proyecto de aprendizaje, CSV es más simple. No requiere instalación adicional y es fácil de entender. En producción, sí usaríamos una base de datos.

**P: ¿Es seguro guardar el PIN en texto plano?**
R: No, no es seguro para producción. Es solo para aprendizaje. En un sistema real, el PIN debería estar encriptado con hash.

**P: ¿Qué pasa si dos personas usan el programa al mismo tiempo?**
R: El programa no está diseñado para múltiples usuarios simultáneos. Podría corromper los archivos. En producción, usaríamos una base de datos con transacciones.

**P: ¿Cómo puedo agregar más funcionalidades?**
R: Puedes agregar nuevas opciones al menú, crear nuevos métodos en la clase Cajero, y actualizar la interfaz en Program.cs.

---

## 15. CONCLUSIÓN

Este proyecto demuestra:
- **Fundamentos de programación**: Variables, bucles, condicionales
- **POO básica**: Clases, objetos, métodos
- **Manejo de archivos**: Lectura y escritura de datos
- **Validación de entrada**: Verificar datos del usuario
- **Organización de código**: Separar responsabilidades

Es un excelente punto de partida para entender cómo funcionan las aplicaciones reales, aunque simplificado para fines educativos.
