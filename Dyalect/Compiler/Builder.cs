﻿using Dyalect.Debug;
using Dyalect.Linker;
using Dyalect.Parser;
using Dyalect.Parser.Model;
using System;
using System.Collections.Generic;
using static Dyalect.Compiler.Hints;

namespace Dyalect.Compiler
{
    internal sealed partial class Builder
    {
        private const int ERROR_LIMIT = 100;

        private readonly bool iterative; //Построение идёт в интерактивном режиме
        private readonly BuilderOptions options; //Настройки сборки
        private readonly CodeWriter cw; //Хелпер для эмита
        private readonly DyLinker linker; //Линкер, который используется для пристыковки модулей
        private Scope globalScope; //Глобальный скоуп для текущего фрейма
        private Unit unit; //Фрейм, который мы сейчас компилируем
        private Scope currentScope; //Текущий лексический скоуп
        private Label programEnd; //Лейбл, отмечающий конец программы
        private Dictionary<string, UnitInfo> referencedUnits = new Dictionary<string, UnitInfo>();
        private Dictionary<string, TypeInfo> types = new Dictionary<string, TypeInfo>();

        private readonly static DImport defaultInclude = new DImport(default) { ModuleName = "lang" };

        public Builder(BuilderOptions options, DyLinker linker)
        {
            this.options = options;
            this.linker = linker;
            counters = new Stack<int>();
            pdb = new DebugWriter();
            isDebug = options.Debug;
            globalScope = new Scope(true, null);
            unit = new Unit
            {
                GlobalScope = globalScope,
                Symbols = pdb.Symbols
            };
            cw = new CodeWriter(unit);
            currentScope = globalScope;
            programEnd = cw.DefineLabel();
        }

        public Builder(Builder builder)
        {
            this.iterative = true;
            this.linker = builder.linker;
            this.imports = builder.imports;
            this.types = builder.types;
            this.referencedUnits = builder.referencedUnits;
            this.counters = new Stack<int>();
            this.options = builder.options;
            this.pdb = builder.pdb.Clone();
            this.unit = builder.unit.Clone(this.pdb.Symbols);
            this.cw = builder.cw.Clone(this.unit);
            this.globalScope = unit.GlobalScope;
            this.currentScope = builder.currentScope != builder.globalScope
                ? builder.currentScope.Clone() : this.globalScope;
            this.isDebug = builder.isDebug;
            this.lastLocation = builder.lastLocation;
            this.Messages = new List<BuildMessage>();
            this.counters = new Stack<int>(builder.counters.ToArray());
            this.currentCounter = builder.currentCounter;
            this.programEnd = cw.DefineLabel();
        }

        public Unit Build(DyCodeModel codeModel)
        {
            Messages.Clear();
            unit.FileName = codeModel.FileName;

            if (unit.Layouts.Count == 0)
                unit.Layouts.Add(null); //Разбивка для топ левела

            cw.StartFrame(0); //Начинаем новый глобальный фрейм
            var res = TryBuild(codeModel.Root);

            if (!res)
                return null;

            cw.MarkLabel(programEnd);
            cw.Term(); //Программка всегда должна завершаться этим оп кодом
            cw.CompileOpList();

            //Завершаем компиляцию, фиксим разбивку памяти для топ левела
            unit.Layouts[0] = new MemoryLayout(currentCounter, cw.FinishFrame(), 0);
            return unit;
        }

        private bool TryBuild(DBlock root)
        {
            try
            {
                var ctx = new CompilerContext();

                if (!options.NoLangModule && !iterative)
                    Build(defaultInclude, None, ctx);

                for (var i = 0; i < root.Nodes.Count; i++)
                {
                    var n = root.Nodes[i];
                    var last = i == root.Nodes.Count - 1;
                    Build(n, last ? Push : None, ctx);
                }

                return Success;
            }
            catch (TerminationException)
            {
                return false;
            }
#if !DEBUG
            catch (Exception ex)
            {
                throw Ice(ex);
            }
#endif
        }

        //Если некий блок кода обычно возвращает значение, но в данном контексте используется
        //как выражение нам нужно снять вычисленное им значение со стека.
        private void PopIf(Hints hints)
        {
            if (!hints.Has(Push))
                cw.Pop();
        }

        //Если некий блок кода значение не возвращает сам по себе, но в данном контексте
        //используется как выражение, мы эмулируем поведение выражения, поднимая на стек NIL
        private void PushIf(Hints hints)
        {
            if (hints.Has(Push))
                cw.PushNil();
        }

        private Exception Ice(Exception ex = null)
        {
            return new DyBuildException(
                $"Внутренняя ошибка компилятора: {(ex != null ? ex.Message : "Недопустимая операция.")}", ex);
        }
    }
}
