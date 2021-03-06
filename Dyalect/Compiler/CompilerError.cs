﻿namespace Dyalect.Compiler
{
    public enum CompilerError
    {
        None = 0,

        TooManyErrors = 201,

        VariableAlreadyDeclared = 202,

        UndefinedVariable = 203,

        UnableAssignExpression = 204,

        UnableAssignConstant = 205,

        NoEnclosingLoop = 206,

        UndefinedType = 207,

        UndefinedModule = 208,

        ReturnNotAllowed = 209,

        InvalidNameOfOperator = 210,

        ExpressionNoName = 211,

        InvalidTypeOfOperator = 212
    }
}
