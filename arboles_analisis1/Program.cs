using System;

namespace arboles_analisis1
{
    internal class Program
    {
        // Esta funcion evalua el arbol
        public static double Evaluar(Nodo nodo)
        {
            // Si no tiene hijos, es un numero
            if (nodo.izquierda == null && nodo.derecha == null)
            {
                return double.Parse(nodo.valor);
            }

            // Si si tiene hijos, evaluo a los dos
            double izquierda = Evaluar(nodo.izquierda);
            double derecha = Evaluar(nodo.derecha);

            // Hago la operacion segun el signo
            switch (nodo.valor)
            {
                case "+": return izquierda + derecha;
                case "-": return izquierda - derecha;
                case "*": return izquierda * derecha;
                case "/":
                    if (derecha == 0)
                        throw new ArithmeticException("No se puede dividir entre cero");
                    return izquierda / derecha;
                default:
                    throw new ArgumentException("Operador desconocido: " + nodo.valor);
            }
        }

        // Esta funcion arma el arbol desde el texto
        static Nodo ConstruirArbol(string expr)
        {
            // Quito espacios
            expr = expr.Replace(" ", "");

            // Si toda la expresion esta entre parentesis, los quito
            // Ejemplo: (3+5) -> 3+5
            while (expr.StartsWith("(") && expr.EndsWith(")") && ParentesisBien(expr))
            {
                expr = expr.Substring(1, expr.Length - 2);
            }

            // Busco el signo mas debil, primero + y -
            // Lo busco de derecha a izquierda para que respete el orden
            int pos = -1;
            int nivel = 0; // Para no contar los que estan dentro de ( )

            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == ')') nivel++;
                if (expr[i] == '(') nivel--;

                // Solo me fijo si no estoy dentro de un parentesis
                if (nivel == 0 && (expr[i] == '+' || expr[i] == '-'))
                {
                    // El - al principio es negativo, no es operacion
                    if (i == 0) continue;
                    pos = i;
                    break;
                }
            }

            // Si no encontre + o -, busco * o /
            if (pos == -1)
            {
                nivel = 0;
                for (int i = expr.Length - 1; i >= 0; i--)
                {
                    if (expr[i] == ')') nivel++;
                    if (expr[i] == '(') nivel--;

                    if (nivel == 0 && (expr[i] == '*' || expr[i] == '/'))
                    {
                        pos = i;
                        break;
                    }
                }
            }

            // Si no encontre ningun signo, es solo un numero
            if (pos == -1)
            {
                return new Nodo(expr);
            }

            // Si si encontre, parto la expresion en 2
            string izq = expr.Substring(0, pos);
            string signo = expr[pos].ToString();
            string der = expr.Substring(pos + 1);

            // Armo el nodo con ese signo
            Nodo raiz = new Nodo(signo);
            raiz.izquierda = ConstruirArbol(izq);
            raiz.derecha = ConstruirArbol(der);

            return raiz;
        }

        // Reviso que los parentesis de afuera si sean pareja
        static bool ParentesisBien(string expr)
        {
            int nivel = 0;
            for (int i = 0; i < expr.Length - 1; i++)
            {
                if (expr[i] == '(') nivel++;
                if (expr[i] == ')') nivel--;
                if (nivel == 0) return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Calculadora de expresiones");
            Console.WriteLine("Ejemplo: (3+5)*2");
            Console.WriteLine("");

            while (true)
            {
                Console.Write("Ingresa la expresion: ");
                string expr = Console.ReadLine();

                if (expr.ToLower() == "salir") break;
                if (expr == "") continue;

                try
                {
                    // Armo el arbol y lo evaluo
                    Nodo raiz = ConstruirArbol(expr);
                    Console.WriteLine(expr + " = " + Evaluar(raiz));
                    Console.WriteLine("");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Presiona cualquier tecla para salir :)");
            Console.ReadKey();
        }
    }
}
