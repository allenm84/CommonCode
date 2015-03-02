using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public class ShuntingYard
  {
    /// <summary>
    /// operators
    /// precedence   operators       associativity
    /// 1            !               right to left
    /// 2            * / %           left to right
    /// 3            + -             left to right
    /// 4            =               right to left
    /// </summary>
    private int op_preced(char c)
    {
      switch (c)
      {
        case '!':
          return 4;
        case '*':
        case '/':
        case '%':
          return 3;
        case '+':
        case '-':
          return 2;
        case '=':
          return 1;
      }
      return 0;
    }

    private bool op_left_assoc(char c)
    {
      switch (c)
      {
          // left to right
        case '*':
        case '/':
        case '%':
        case '+':
        case '-':
          return true;
          // right to left
        case '=':
        case '!':
          return false;
      }
      return false;
    }

    private int op_arg_count(char c)
    {
      switch (c)
      {
        case '*':
        case '/':
        case '%':
        case '+':
        case '-':
        case '=':
          return 2;
        case '!':
          return 1;
        default:
          return c - 'A';
      }
    }

    private bool is_operator(char c)
    {
      return (c == '+' || c == '-' || c == '/' || c == '*' || c == '!' || c == '%' || c == '=');
    }

    private bool is_function(char c)
    {
      return (c >= 'A' && c <= 'Z');
    }

    private bool is_ident(char c)
    {
      return ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'));
    }

    public bool shunting_yard(char[] input, out List<char> output)
    {
      // operator stack
      Stack<char> stack = new Stack<char>(32);

      // used for record stack element
      char sc;

      // a list of chars
      output = new List<char>();

      foreach (var c in input)
      {
        // read one token from the input stream
        if (c != ' ')
        {
          // If the token is a number (identifier), then add it to the output queue.
          if (is_ident(c))
          {
            output.Add(c);
          }
            // If the token is a function token, then push it onto the stack.
          else if (is_function(c))
          {
            stack.Push(c);
          }
            // If the token is a function argument separator (e.g., a comma):
          else if (c == ',')
          {
            bool pe = false;
            while (stack.Count > 0)
            {
              sc = stack.Pop();
              if (sc == '(')
              {
                pe = true;
                break;
              }
              else
              {
                // Until the token at the top of the stack is a left parenthesis,
                // pop operators off the stack onto the output queue.
                output.Add(sc);
              }
            }
            // If no left parentheses are encountered, either the separator was misplaced
            // or parentheses were mismatched.
            if (!pe)
            {
              Console.Write("Error: separator or parentheses mismatched\n");
              return false;
            }
          }
            // If the token is an operator, op1, then:
          else if (is_operator(c))
          {
            while (stack.Count > 0)
            {
              sc = stack.Pop();
              // While there is an operator token, o2, at the top of the stack
              // op1 is left-associative and its precedence is less than or equal to that of op2,
              // or op1 is right-associative and its precedence is less than that of op2,
              if (is_operator(sc) &&
                ((op_left_assoc(c) && (op_preced(c) <= op_preced(sc))) ||
                  (!op_left_assoc(c) && (op_preced(c) < op_preced(sc)))))
              {
                // Pop o2 off the stack, onto the output queue;
                output.Add(sc);
              }
              else
              {
                break;
              }
            }
            // push op1 onto the stack.
            stack.Push(c);
          }
            // If the token is a left parenthesis, then push it onto the stack.
          else if (c == '(')
          {
            stack.Push(c);
          }
            // If the token is a right parenthesis:
          else if (c == ')')
          {
            bool pe = false;
            // Until the token at the top of the stack is a left parenthesis,
            // pop operators off the stack onto the output queue
            while (stack.Count > 0)
            {
              sc = stack.Pop();
              if (sc == '(')
              {
                pe = true;
                break;
              }
              else
              {
                output.Add(sc);
              }
            }

            // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
            if (!pe)
            {
              Console.Write("Error: parentheses mismatched\n");
              return false;
            }

            // Pop the left parenthesis from the stack, but not onto the output queue.
            stack.Pop();

            // If the token at the top of the stack is a function token, pop it onto the output queue.
            if (stack.Count > 0)
            {
              sc = stack.Pop();
              if (is_function(sc))
              {
                output.Add(sc);
              }
            }
          }
          else
          {
            Console.Write("Unknown token {0}\n", c);
            return false; // Unknown token
          }
        }
      }

      // When there are no more tokens to read:
      // While there are still operator tokens in the stack:
      while (stack.Count > 0)
      {
        sc = stack.Pop();
        if (sc == '(' || sc == ')')
        {
          Console.Write("Error: parentheses mismatched\n");
          return false;
        }
        output.Add(sc);
      }
      return true;
    }

    public bool execution_order(char[] input, out char result)
    {
      char sc;
      Stack<char> stack = new Stack<char>(32);
      result = '-';

      // While there are input tokens left
      foreach (var c in input)
      {
        // If the token is a value or identifier
        if (is_ident(c))
        {
          // Push it onto the stack.
          stack.Push(c);
        }
          // Otherwise, the token is an operator  (operator here includes both operators, and functions).
        else if (is_operator(c) || is_function(c))
        {
          // It is known a priori that the operator takes n arguments.
          int nargs = op_arg_count(c);

          // If there are fewer than n values on the stack
          if (stack.Count < nargs)
          {
            Console.WriteLine("Error: The user has not input sufficient values in the expression.");
            return false;
          }
          // Else, Pop the top n values from the stack.
          // Evaluate the operator, with the values as arguments.
          if (is_function(c))
          {
            while (nargs > 0)
            {
              sc = stack.Pop();
              --nargs;
            }
          }
          else
          {
            if (nargs == 1)
            {
              sc = stack.Pop();
            }
            else
            {
              sc = stack.Pop();
              sc = stack.Pop();
            }
          }
        }
      }

      // If there is only one value in the stack
      // That value is the result of the calculation.
      if (stack.Count == 1)
      {
        sc = stack.Pop();
        result = sc;
        return true;
      }

      // If there are more values in the stack
      // (Error) The user input has too many values.
      return false;
    }
  }
}
