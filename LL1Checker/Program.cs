using System;

namespace LL1Checker
{
	internal class Program
	{
		public static void Main()
		{
			//Calc1();
			//Calc2();
			//Example();
			PL0p();
		}

		private static void Calc1()
		{
			/*
			 * Grammer
			 * 
			 * <Expr>   ::= <Term>   | <Expr> '+' <Term>   | <Expr> '-' <Term>
			 * <Term>   ::= <Factor> | <Term> '*' <Factor> | <Term> '/' <Factor>
			 * <Factor> ::= Num      | '(' <Expr> ')'
			*/

			Grammer grammer = new();

			//////////////////////////////
			// terminal symbols
			//////////////////////////////
			SymbolPool symbolPool = new();
			Symbol symbolLparen = symbolPool.GetSymbol(@"lparen");
			grammer.AddRule(symbolLparen, TokenType.PunctuationLeftParen);
			Symbol symbolRparen = symbolPool.GetSymbol(@"rparen");
			grammer.AddRule(symbolRparen, TokenType.PunctuationRightParen);
			Symbol symbolPlus = symbolPool.GetSymbol(@"plus");
			grammer.AddRule(symbolPlus, TokenType.PunctuationPlus);
			Symbol symbolMinus = symbolPool.GetSymbol(@"minus");
			grammer.AddRule(symbolMinus, TokenType.PunctuationMinus);
			Symbol symbolAsterisk = symbolPool.GetSymbol(@"asterisk");
			grammer.AddRule(symbolAsterisk, TokenType.PunctuationAsterisk);
			Symbol symbolSlash = symbolPool.GetSymbol(@"slash");
			grammer.AddRule(symbolSlash, TokenType.PunctuationSlash);
			Symbol symbolIntnum = symbolPool.GetSymbol(@"intnum");
			grammer.AddRule(symbolIntnum, TokenType.LiteralInteger);

			//////////////////////////////
			// non-terminal
			//////////////////////////////
			Symbol symbolExpression = symbolPool.GetSymbol(@"expression");
			Symbol symbolTerm = symbolPool.GetSymbol(@"term");
			Symbol symbolFactor = symbolPool.GetSymbol(@"factor");

			grammer.AddRule(symbolExpression, new Symbol[] { symbolTerm });
			grammer.AddRule(symbolExpression, new Symbol[] { symbolExpression, symbolPlus, symbolTerm });
			grammer.AddRule(symbolExpression, new Symbol[] { symbolExpression, symbolMinus, symbolTerm });

			grammer.AddRule(symbolTerm, new Symbol[] { symbolFactor });
			grammer.AddRule(symbolTerm, new Symbol[] { symbolTerm, symbolAsterisk, symbolFactor });
			grammer.AddRule(symbolTerm, new Symbol[] { symbolTerm, symbolSlash, symbolFactor });

			grammer.AddRule(symbolFactor, new Symbol[] { symbolIntnum });
			grammer.AddRule(symbolFactor, new Symbol[] { symbolLparen, symbolExpression, symbolRparen });

			grammer.SetStartSymbol(symbolExpression);

			bool isLL1Grammer = grammer.CheckWhetherLL1OrNot();
			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}

		private static void Calc2()
		{
			/*
			 * Grammer
			 * 
			 * <Expr>   ::= <Term> <Expr2>
			 * <Expr2>  ::= '+' <Term> <Expr2> | '-' <Term> <Expr2> | ε
			 * <Term>   ::= <Factor> <Term2>
			 * <Term2>  ::= * <Factor> <Term2> | '/' <Factor> <Term2> | ε
			 * <Factor> ::= Num | ( <Expr> )
			*/

			Grammer grammer = new();

			//////////////////////////////
			// terminal symbols
			//////////////////////////////
			SymbolPool symbolPool = new();
			Symbol symbolLparen = symbolPool.GetSymbol(@"lparen");
			grammer.AddRule(symbolLparen, TokenType.PunctuationLeftParen);
			Symbol symbolRparen = symbolPool.GetSymbol(@"rparen");
			grammer.AddRule(symbolRparen, TokenType.PunctuationRightParen);
			Symbol symbolPlus = symbolPool.GetSymbol(@"plus");
			grammer.AddRule(symbolPlus, TokenType.PunctuationPlus);
			Symbol symbolMinus = symbolPool.GetSymbol(@"minus");
			grammer.AddRule(symbolMinus, TokenType.PunctuationMinus);
			Symbol symbolAsterisk = symbolPool.GetSymbol(@"asterisk");
			grammer.AddRule(symbolAsterisk, TokenType.PunctuationAsterisk);
			Symbol symbolSlash = symbolPool.GetSymbol(@"slash");
			grammer.AddRule(symbolSlash, TokenType.PunctuationSlash);
			Symbol symbolIntnum = symbolPool.GetSymbol(@"intnum");
			grammer.AddRule(symbolIntnum, TokenType.LiteralInteger);

			//////////////////////////////
			// non-terminal
			//////////////////////////////
			Symbol symbolExpression = symbolPool.GetSymbol(@"expression");
			Symbol symbolExpression2 = symbolPool.GetSymbol(@"expression2");
			Symbol symbolTerm = symbolPool.GetSymbol(@"term");
			Symbol symbolTerm2 = symbolPool.GetSymbol(@"term2");
			Symbol symbolFactor = symbolPool.GetSymbol(@"factor");

			grammer.AddRule(symbolExpression, new Symbol[] { symbolTerm, symbolExpression2 });
			grammer.AddRule(symbolExpression2, new Symbol[] { symbolPlus, symbolTerm, symbolExpression2 });
			grammer.AddRule(symbolExpression2, new Symbol[] { symbolMinus, symbolTerm, symbolExpression2 });
			grammer.AddRule(symbolExpression2, new Symbol[] { SymbolPool.Empty });

			grammer.AddRule(symbolTerm, new Symbol[] { symbolFactor, symbolTerm2 });
			grammer.AddRule(symbolTerm2, new Symbol[] { symbolAsterisk, symbolFactor, symbolTerm2 });
			grammer.AddRule(symbolTerm2, new Symbol[] { symbolSlash, symbolFactor, symbolTerm2 });
			grammer.AddRule(symbolTerm2, new Symbol[] { SymbolPool.Empty });

			grammer.AddRule(symbolFactor, new Symbol[] { symbolIntnum });
			grammer.AddRule(symbolFactor, new Symbol[] { symbolLparen, symbolExpression, symbolRparen });

			grammer.SetStartSymbol(symbolExpression);

			bool isLL1Grammer = grammer.CheckWhetherLL1OrNot();
			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}

		private static void Example()
		{
			/*
			 * Grammer
			 * 
			 * <Expr>   ::= <Term> <Expr2>
			 * <Expr2>  ::= '+' <Term> <Expr2> | ε
			 * <Term>   ::= <Factor> <Term2>
			 * <Term2>  ::= '*' <Factor> <Term2> | ε
			 * <Factor> ::= Num | ( <Expr> )
			*/

			Grammer grammer = new();

			//////////////////////////////
			// terminal symbols
			//////////////////////////////
			SymbolPool symbolPool = new();
			Symbol symbolLparen = symbolPool.GetSymbol(@"lparen");
			grammer.AddRule(symbolLparen, TokenType.PunctuationLeftParen);
			Symbol symbolRparen = symbolPool.GetSymbol(@"rparen");
			grammer.AddRule(symbolRparen, TokenType.PunctuationRightParen);
			Symbol symbolPlus = symbolPool.GetSymbol(@"plus");
			grammer.AddRule(symbolPlus, TokenType.PunctuationPlus);
			Symbol symbolAsterisk = symbolPool.GetSymbol(@"asterisk");
			grammer.AddRule(symbolAsterisk, TokenType.PunctuationAsterisk);
			Symbol symbolIntnum = symbolPool.GetSymbol(@"intnum");
			grammer.AddRule(symbolIntnum, TokenType.LiteralInteger);

			//////////////////////////////
			// non-terminal
			//////////////////////////////
			Symbol symbolExpression = symbolPool.GetSymbol(@"expression");
			Symbol symbolExpression2 = symbolPool.GetSymbol(@"expression2");
			Symbol symbolTerm = symbolPool.GetSymbol(@"term");
			Symbol symbolTerm2 = symbolPool.GetSymbol(@"term2");
			Symbol symbolFactor = symbolPool.GetSymbol(@"factor");

			grammer.AddRule(symbolExpression, new Symbol[] { symbolTerm, symbolExpression2 });
			grammer.AddRule(symbolExpression2, new Symbol[] { symbolPlus, symbolTerm, symbolExpression2 });
			grammer.AddRule(symbolExpression2, new Symbol[] { SymbolPool.Empty });

			grammer.AddRule(symbolTerm, new Symbol[] { symbolFactor, symbolTerm2 });
			grammer.AddRule(symbolTerm2, new Symbol[] { symbolAsterisk, symbolFactor, symbolTerm2 });
			grammer.AddRule(symbolTerm2, new Symbol[] { SymbolPool.Empty });

			grammer.AddRule(symbolFactor, new Symbol[] { symbolIntnum });
			grammer.AddRule(symbolFactor, new Symbol[] { symbolLparen, symbolExpression, symbolRparen });

			grammer.SetStartSymbol(symbolExpression);

			bool isLL1Grammer = grammer.CheckWhetherLL1OrNot();
			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}

		/// <summary>
		/// </summary>
		/// <see cref="https://www.ohmsha.co.jp/book/9784274221163/"/>
		private static void PL0p()
		{
			Grammer grammer = new();
			SymbolPool symbolPool = new();

			////////////////////////////////////////
			// terminal symbols
			////////////////////////////////////////
			// keywords
			Symbol symbolConst = symbolPool.GetSymbol(@"const");
			Symbol symbolVar = symbolPool.GetSymbol(@"var");
			Symbol symbolFunction = symbolPool.GetSymbol(@"function");
			Symbol symbolBegin = symbolPool.GetSymbol(@"begin");
			Symbol symbolEnd = symbolPool.GetSymbol(@"end");
			Symbol symbolIf = symbolPool.GetSymbol(@"if");
			Symbol symbolThen = symbolPool.GetSymbol(@"then");
			Symbol symbolWhile = symbolPool.GetSymbol(@"while");
			Symbol symbolDo = symbolPool.GetSymbol(@"do");
			Symbol symbolReturn = symbolPool.GetSymbol(@"return");
			// name
			Symbol symbolWrite = symbolPool.GetSymbol(@"write");
			Symbol symbolWriteln = symbolPool.GetSymbol(@"writeln");
			Symbol symbolOdd = symbolPool.GetSymbol(@"odd");
			// punctuation marks
			Symbol symbolPeriod = symbolPool.GetSymbol(@"period");
			Symbol symbolEqual = symbolPool.GetSymbol(@"equal");
			Symbol symbolSemicolon = symbolPool.GetSymbol(@"semicolon");
			Symbol symbolComma = symbolPool.GetSymbol(@"comma");
			Symbol symbolLparen = symbolPool.GetSymbol(@"lparen");
			Symbol symbolRparen = symbolPool.GetSymbol(@"rparen");
			Symbol symbolPlus = symbolPool.GetSymbol(@"plus");
			Symbol symbolMinus = symbolPool.GetSymbol(@"minus");
			Symbol symbolAsterisk = symbolPool.GetSymbol(@"asterisk");
			Symbol symbolSlash = symbolPool.GetSymbol(@"slash");
			Symbol symbolColonEqual = symbolPool.GetSymbol(@"colonEqual");
			Symbol symbolLTGT = symbolPool.GetSymbol(@"ltgt");
			Symbol symbolLT = symbolPool.GetSymbol(@"lt");
			Symbol symbolGT = symbolPool.GetSymbol(@"gt");
			Symbol symbolLE = symbolPool.GetSymbol(@"le");
			Symbol symbolGE = symbolPool.GetSymbol(@"ge");
			Symbol symbolIntnum = symbolPool.GetSymbol(@"intnum");
			Symbol symbolId = symbolPool.GetSymbol(@"id");

			////////////////////////////////////////
			// non-terminal
			////////////////////////////////////////
			Symbol symbolProgram = symbolPool.GetSymbol(@"program");
			Symbol symbolDecl = symbolPool.GetSymbol(@"decl");
			Symbol symbolDecls = symbolPool.GetSymbol(@"decls");
			Symbol symbolBlock = symbolPool.GetSymbol(@"block");
			Symbol symbolConstEntry = symbolPool.GetSymbol(@"constEntry");
			Symbol symbolConstEntries = symbolPool.GetSymbol(@"constEntries");
			Symbol symbolConstEntriesTail = symbolPool.GetSymbol(@"constEntriesTail");
			Symbol symbolConstDecl = symbolPool.GetSymbol(@"constDecl");
			Symbol symbolVariables = symbolPool.GetSymbol(@"variables");
			Symbol symbolVariablesTail = symbolPool.GetSymbol(@"variablesTail");
			Symbol symbolVarDecl = symbolPool.GetSymbol(@"varDecl");
			Symbol symbolVariables0 = symbolPool.GetSymbol(@"variables0");
			Symbol symbolFuncDecl = symbolPool.GetSymbol(@"funcDecl");
			Symbol symbolAssignment = symbolPool.GetSymbol(@"assignment");
			Symbol symbolStatements = symbolPool.GetSymbol(@"statements");
			Symbol symbolStatementsTail = symbolPool.GetSymbol(@"statementsTail");
			Symbol symbolBeginEnd = symbolPool.GetSymbol(@"beginEnd");
			Symbol symbolIfStatement = symbolPool.GetSymbol(@"ifStatement");
			Symbol symbolWhileStatement = symbolPool.GetSymbol(@"whileStatement");
			Symbol symbolReturnStatement = symbolPool.GetSymbol(@"returnStatement");
			Symbol symbolWriteStatement = symbolPool.GetSymbol(@"writeStatement");
			Symbol symbolStatement = symbolPool.GetSymbol(@"statement");
			Symbol symbolBoolOperator = symbolPool.GetSymbol(@"boolOperator");
			Symbol symbolCondition = symbolPool.GetSymbol(@"condition");
			Symbol symbolAddsub = symbolPool.GetSymbol(@"addsub");
			Symbol symbolAddsub0 = symbolPool.GetSymbol(@"addsub0");
			Symbol symbolExpression = symbolPool.GetSymbol(@"expression");
			Symbol symbolExpressionTail = symbolPool.GetSymbol(@"expressionTail");
			Symbol symbolMuldiv = symbolPool.GetSymbol(@"muldiv");
			Symbol symbolTermTail = symbolPool.GetSymbol(@"termTail");
			Symbol symbolTerm = symbolPool.GetSymbol(@"term");
			Symbol symbolExpressions = symbolPool.GetSymbol(@"expressions");
			Symbol symbolExpressionsTail = symbolPool.GetSymbol(@"expressionsTail");
			Symbol symbolExpressions0 = symbolPool.GetSymbol(@"expressions0");
			Symbol symbolFactor = symbolPool.GetSymbol(@"factor");
			Symbol symbolArguments = symbolPool.GetSymbol(@"arguments");

			////////////////////////////////////////
			// production
			////////////////////////////////////////
			// program
			grammer.AddRule(symbolProgram, new Symbol[] { symbolBlock, symbolPeriod });
			// decl
			grammer.AddRule(symbolDecl, new Symbol[] { symbolConstDecl });
			grammer.AddRule(symbolDecl, new Symbol[] { symbolVarDecl });
			grammer.AddRule(symbolDecl, new Symbol[] { symbolFuncDecl });
			// decls
			grammer.AddRule(symbolDecls, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolDecls, new Symbol[] { symbolDecl, symbolDecls });
			// block
			grammer.AddRule(symbolBlock, new Symbol[] { symbolDecls, symbolStatement });
			// constEntry
			grammer.AddRule(symbolConstEntry, new Symbol[] { symbolId, symbolEqual, symbolIntnum });
			// constEntries
			grammer.AddRule(symbolConstEntries, new Symbol[] { symbolConstEntry, symbolConstEntriesTail });
			// constEntriesTail
			grammer.AddRule(symbolConstEntriesTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolConstEntriesTail, new Symbol[] { symbolComma, symbolConstEntry, symbolConstEntriesTail });
			// constDecl
			grammer.AddRule(symbolConstDecl, new Symbol[] { symbolConst, symbolConstEntries, symbolSemicolon });
			// variables
			grammer.AddRule(symbolVariables, new Symbol[] { symbolId, symbolVariablesTail });
			// variablesTail
			grammer.AddRule(symbolVariablesTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolVariablesTail, new Symbol[] { symbolComma, symbolId, symbolVariablesTail });
			// varDecl
			grammer.AddRule(symbolVarDecl, new Symbol[] { symbolVar, symbolVariables, symbolSemicolon });
			// variables0
			grammer.AddRule(symbolVariables0, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolVariables0, new Symbol[] { symbolVariables });
			// funcDecl
			grammer.AddRule(symbolFuncDecl, new Symbol[] { symbolFunction, symbolId, symbolLparen, symbolVariables0, symbolRparen, symbolBlock, symbolSemicolon });
			// assignment
			grammer.AddRule(symbolAssignment, new Symbol[] { symbolId, symbolColonEqual, symbolExpression });
			// statements
			grammer.AddRule(symbolStatements, new Symbol[] { symbolStatement, symbolStatementsTail });
			// statementsTail
			grammer.AddRule(symbolStatementsTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolStatementsTail, new Symbol[] { symbolComma, symbolStatement, symbolStatementsTail });
			// beginEnd
			grammer.AddRule(symbolBeginEnd, new Symbol[] { symbolBegin, symbolStatements, symbolEnd });
			// ifStatement
			grammer.AddRule(symbolIfStatement, new Symbol[] { symbolIf, symbolCondition, symbolThen, symbolStatement });
			// whileStatement
			grammer.AddRule(symbolWhileStatement, new Symbol[] { symbolWhile, symbolCondition, symbolDo, symbolStatement });
			// returnStatement
			grammer.AddRule(symbolReturnStatement, new Symbol[] { symbolReturn, symbolExpression });
			// writeStatement
			grammer.AddRule(symbolWriteStatement, new Symbol[] { symbolWrite, symbolExpression });
			// statement
			grammer.AddRule(symbolStatement, new Symbol[] { symbolAssignment });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolBeginEnd });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolIfStatement });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolWhileStatement });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolReturnStatement });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolWriteStatement });
			grammer.AddRule(symbolStatement, new Symbol[] { symbolWriteln });
			// boolOperator
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolEqual });
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolLTGT });
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolLT });
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolGT });
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolLE });
			grammer.AddRule(symbolBoolOperator, new Symbol[] { symbolGE });
			// condition
			grammer.AddRule(symbolCondition, new Symbol[] { symbolOdd, symbolExpression });
			grammer.AddRule(symbolCondition, new Symbol[] { symbolExpression, symbolBoolOperator, symbolExpression });
			// addsub
			grammer.AddRule(symbolAddsub, new Symbol[] { symbolPlus });
			grammer.AddRule(symbolAddsub, new Symbol[] { symbolMinus });
			// addsub0
			grammer.AddRule(symbolAddsub0, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolAddsub0, new Symbol[] { symbolAddsub });
			// expression
			grammer.AddRule(symbolExpression, new Symbol[] { symbolAddsub0, symbolTerm, symbolExpressionTail });
			// expressionTail
			grammer.AddRule(symbolExpressionTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolExpressionTail, new Symbol[] { symbolAddsub, symbolTerm, symbolExpressionTail });
			// muldiv
			grammer.AddRule(symbolMuldiv, new Symbol[] { symbolAsterisk });
			grammer.AddRule(symbolMuldiv, new Symbol[] { symbolSlash });
			// term2
			grammer.AddRule(symbolTermTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolTermTail, new Symbol[] { symbolMuldiv, symbolFactor, symbolTermTail });
			// term
			grammer.AddRule(symbolTerm, new Symbol[] { symbolFactor, symbolTermTail });
			// expressions
			grammer.AddRule(symbolExpressions, new Symbol[] { symbolExpression, symbolExpressionsTail });
			// expressionsTail
			grammer.AddRule(symbolExpressionsTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolExpressionsTail, new Symbol[] { symbolComma, symbolExpression, symbolExpressionsTail });
			// expressions0
			grammer.AddRule(symbolExpressions0, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolExpressions0, new Symbol[] { symbolExpressions });
			// arguments
			grammer.AddRule(symbolArguments, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(symbolArguments, new Symbol[] { symbolLparen, symbolExpressions0, symbolRparen });
			// factor
			grammer.AddRule(symbolFactor, new Symbol[] { symbolId, symbolArguments });
			grammer.AddRule(symbolFactor, new Symbol[] { symbolIntnum });
			grammer.AddRule(symbolFactor, new Symbol[] { symbolLparen, symbolExpression, symbolRparen });
			// nonterminal symbols
			grammer.AddRule(symbolConst, TokenType.KeywordConst);
			grammer.AddRule(symbolVar, TokenType.KeywordVar);
			grammer.AddRule(symbolFunction, TokenType.KeywordFunction);
			grammer.AddRule(symbolBegin, TokenType.KeywordBegin);
			grammer.AddRule(symbolEnd, TokenType.KeywordEnd);
			grammer.AddRule(symbolIf, TokenType.KeywordIf);
			grammer.AddRule(symbolThen, TokenType.KeywordThen);
			grammer.AddRule(symbolWhile, TokenType.KeywordWhile);
			grammer.AddRule(symbolDo, TokenType.KeywordDo);
			grammer.AddRule(symbolReturn, TokenType.KeywordReturn);
			grammer.AddRule(symbolWrite, TokenType.NameWrite);
			grammer.AddRule(symbolWriteln, TokenType.NameWriteln);
			grammer.AddRule(symbolOdd, TokenType.NameOdd);
			grammer.AddRule(symbolPeriod, TokenType.PunctuationPeriod);
			grammer.AddRule(symbolEqual, TokenType.PunctuationEqual);
			grammer.AddRule(symbolSemicolon, TokenType.PunctuationSemicolon);
			grammer.AddRule(symbolComma, TokenType.PunctuationComma);
			grammer.AddRule(symbolLparen, TokenType.PunctuationLeftParen);
			grammer.AddRule(symbolRparen, TokenType.PunctuationRightParen);
			grammer.AddRule(symbolPlus, TokenType.PunctuationPlus);
			grammer.AddRule(symbolMinus, TokenType.PunctuationMinus);
			grammer.AddRule(symbolAsterisk, TokenType.PunctuationAsterisk);
			grammer.AddRule(symbolSlash, TokenType.PunctuationSlash);
			grammer.AddRule(symbolColonEqual, TokenType.PunctuationColonEqual);
			grammer.AddRule(symbolLTGT, TokenType.PunctuationLTGT);
			grammer.AddRule(symbolLT, TokenType.PunctuationLT);
			grammer.AddRule(symbolGT, TokenType.PunctuationGT);
			grammer.AddRule(symbolLE, TokenType.PunctuationLE);
			grammer.AddRule(symbolGE, TokenType.PunctuationGE);
			grammer.AddRule(symbolIntnum, TokenType.LiteralInteger);
			grammer.AddRule(symbolId, TokenType.Identifer);

			////////////////////////////////////////
			//　start symbol
			////////////////////////////////////////
			grammer.SetStartSymbol(symbolProgram);

			bool isLL1Grammer = grammer.CheckWhetherLL1OrNot();
			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}
	}
}
