using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LL1Checker
{
	internal class Program
	{
		public static void Main()
		{
			//Calc1();
			//Calc2();
			//Example();
			//PL0p();
			Json();
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

			Grammer<TokenType> grammer = new();

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

			var resultCheck = grammer.CheckWhetherLL1OrNot();
			bool isLL1Grammer = resultCheck.Item1;
			var firstSet = resultCheck.Item2;
			var followSet = resultCheck.Item3;
			var epsilonDerivabilitySet = resultCheck.Item4;
			var directorSet = resultCheck.Item5;
			var overwrappingPairsDic = resultCheck.Item6;

			grammer.DisplayGrammer();
			DisplayFirstSet(firstSet);
			DisplayFollowSet(followSet);
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet);
			DisplayDirectorSet(directorSet);
			DisplayOverwrappingDirectorSets(overwrappingPairsDic);

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

			Grammer<TokenType> grammer = new();

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

			var resultCheck = grammer.CheckWhetherLL1OrNot();
			bool isLL1Grammer = resultCheck.Item1;
			var firstSet = resultCheck.Item2;
			var followSet = resultCheck.Item3;
			var epsilonDerivabilitySet = resultCheck.Item4;
			var directorSet = resultCheck.Item5;
			var overwrappingPairsDic = resultCheck.Item6;

			grammer.DisplayGrammer();
			DisplayFirstSet(firstSet);
			DisplayFollowSet(followSet);
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet);
			DisplayDirectorSet(directorSet);
			DisplayOverwrappingDirectorSets(overwrappingPairsDic);

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

			Grammer<TokenType> grammer = new();

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

			var resultCheck = grammer.CheckWhetherLL1OrNot();
			bool isLL1Grammer = resultCheck.Item1;
			var firstSet = resultCheck.Item2;
			var followSet = resultCheck.Item3;
			var epsilonDerivabilitySet = resultCheck.Item4;
			var directorSet = resultCheck.Item5;
			var overwrappingPairsDic = resultCheck.Item6;

			grammer.DisplayGrammer();
			DisplayFirstSet(firstSet);
			DisplayFollowSet(followSet);
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet);
			DisplayDirectorSet(directorSet);
			DisplayOverwrappingDirectorSets(overwrappingPairsDic);

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
			Grammer<TokenTypePL0p> grammer = new();
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

			// terminal symbols
			grammer.AddRule(symbolConst, TokenTypePL0p.KeywordConst);
			grammer.AddRule(symbolVar, TokenTypePL0p.KeywordVar);
			grammer.AddRule(symbolFunction, TokenTypePL0p.KeywordFunction);
			grammer.AddRule(symbolBegin, TokenTypePL0p.KeywordBegin);
			grammer.AddRule(symbolEnd, TokenTypePL0p.KeywordEnd);
			grammer.AddRule(symbolIf, TokenTypePL0p.KeywordIf);
			grammer.AddRule(symbolThen, TokenTypePL0p.KeywordThen);
			grammer.AddRule(symbolWhile, TokenTypePL0p.KeywordWhile);
			grammer.AddRule(symbolDo, TokenTypePL0p.KeywordDo);
			grammer.AddRule(symbolReturn, TokenTypePL0p.KeywordReturn);
			grammer.AddRule(symbolWrite, TokenTypePL0p.NameWrite);
			grammer.AddRule(symbolWriteln, TokenTypePL0p.NameWriteln);
			grammer.AddRule(symbolOdd, TokenTypePL0p.NameOdd);
			grammer.AddRule(symbolPeriod, TokenTypePL0p.PunctuationPeriod);
			grammer.AddRule(symbolEqual, TokenTypePL0p.PunctuationEqual);
			grammer.AddRule(symbolSemicolon, TokenTypePL0p.PunctuationSemicolon);
			grammer.AddRule(symbolComma, TokenTypePL0p.PunctuationComma);
			grammer.AddRule(symbolLparen, TokenTypePL0p.PunctuationLeftParen);
			grammer.AddRule(symbolRparen, TokenTypePL0p.PunctuationRightParen);
			grammer.AddRule(symbolPlus, TokenTypePL0p.PunctuationPlus);
			grammer.AddRule(symbolMinus, TokenTypePL0p.PunctuationMinus);
			grammer.AddRule(symbolAsterisk, TokenTypePL0p.PunctuationAsterisk);
			grammer.AddRule(symbolSlash, TokenTypePL0p.PunctuationSlash);
			grammer.AddRule(symbolColonEqual, TokenTypePL0p.PunctuationColonEqual);
			grammer.AddRule(symbolLTGT, TokenTypePL0p.PunctuationLTGT);
			grammer.AddRule(symbolLT, TokenTypePL0p.PunctuationLT);
			grammer.AddRule(symbolGT, TokenTypePL0p.PunctuationGT);
			grammer.AddRule(symbolLE, TokenTypePL0p.PunctuationLE);
			grammer.AddRule(symbolGE, TokenTypePL0p.PunctuationGE);
			grammer.AddRule(symbolIntnum, TokenTypePL0p.LiteralInteger);
			grammer.AddRule(symbolId, TokenTypePL0p.Identifer);

			////////////////////////////////////////
			//　start symbol
			////////////////////////////////////////
			grammer.SetStartSymbol(symbolProgram);


			var resultCheck = grammer.CheckWhetherLL1OrNot();
			bool isLL1Grammer = resultCheck.Item1;
			var firstSet = resultCheck.Item2;
			var followSet = resultCheck.Item3;
			var epsilonDerivabilitySet = resultCheck.Item4;
			var directorSet = resultCheck.Item5;
			var overwrappingPairsDic = resultCheck.Item6;

			grammer.DisplayGrammer();
			DisplayFirstSet(firstSet);
			DisplayFollowSet(followSet);
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet);
			DisplayDirectorSet(directorSet);
			DisplayOverwrappingDirectorSets(overwrappingPairsDic);

			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}

		private static void Json()
		{
			Grammer<TokenTypeJson> grammer = new();
			SymbolPool symbolPool = new();

			////////////////////////////////////////
			// terminal symbols
			////////////////////////////////////////
			// { left curly bracket
			Symbol begin_object = symbolPool.GetSymbol(@"{");    
			grammer.AddRule(begin_object, TokenTypeJson.BeginObject);
			// } right curly bracket
			Symbol end_object = symbolPool.GetSymbol(@"}");
			grammer.AddRule(end_object, TokenTypeJson.EndObject);
			// [ left square bracket
			Symbol begin_array = symbolPool.GetSymbol(@"[");     
			grammer.AddRule(begin_array, TokenTypeJson.BeginArray);
			// ] right square bracket
			Symbol end_array = symbolPool.GetSymbol(@"]");
			grammer.AddRule(end_array, TokenTypeJson.EndArray);
			// : colon
			Symbol name_separator = symbolPool.GetSymbol(@":");
			grammer.AddRule(name_separator, TokenTypeJson.NameSeparator);
			// , comma
			Symbol value_separator = symbolPool.GetSymbol(@",");
			grammer.AddRule(value_separator, TokenTypeJson.ValueSeparator);
			// string
			Symbol string_ = symbolPool.GetSymbol("string");
			grammer.AddRule(string_, TokenTypeJson.String);
			// number
			Symbol number = symbolPool.GetSymbol("number");
			grammer.AddRule(number, TokenTypeJson.Number);
			// true
			Symbol true_ = symbolPool.GetSymbol("true");
			grammer.AddRule(true_, TokenTypeJson.True);
			// false
			Symbol false_ = symbolPool.GetSymbol("false");
			grammer.AddRule(false_, TokenTypeJson.False);
			// null
			Symbol null_ = symbolPool.GetSymbol("null");
			grammer.AddRule(null_, TokenTypeJson.Null);

			////////////////////////////////////////
			// non-terminal symbols
			////////////////////////////////////////
			Symbol member = symbolPool.GetSymbol(@"member");
			Symbol members = symbolPool.GetSymbol(@"members");
			Symbol membersTail = symbolPool.GetSymbol(@"membersTail");
			Symbol members0 = symbolPool.GetSymbol(@"membersTail");
			Symbol object_ = symbolPool.GetSymbol(@"object");
			Symbol values = symbolPool.GetSymbol(@"values");
			Symbol valuesTail = symbolPool.GetSymbol(@"valuesTail");
			Symbol values0 = symbolPool.GetSymbol(@"values0");
			Symbol array = symbolPool.GetSymbol(@"array");
			Symbol value = symbolPool.GetSymbol(@"value");

			grammer.AddRule(member, new Symbol[] { string_, name_separator, value });
			grammer.AddRule(members, new Symbol[] { member, membersTail });
			grammer.AddRule(membersTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(membersTail, new Symbol[] { value_separator, member, membersTail });
			grammer.AddRule(members0, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(members0, new Symbol[] { members });
			grammer.AddRule(object_, new Symbol[] { begin_object, members0, end_object });

			grammer.AddRule(values, new Symbol[] { value, valuesTail });
			grammer.AddRule(valuesTail, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(valuesTail, new Symbol[] { value, valuesTail });
			grammer.AddRule(values0, new Symbol[] { SymbolPool.Empty });
			grammer.AddRule(values0, new Symbol[] { values });
			grammer.AddRule(array, new Symbol[] { begin_array, values0, end_array});

			grammer.AddRule(value, new Symbol[] { false_ });
			grammer.AddRule(value, new Symbol[] { true_ });
			grammer.AddRule(value, new Symbol[] { null_ });
			grammer.AddRule(value, new Symbol[] { object_ });
			grammer.AddRule(value, new Symbol[] { array });
			grammer.AddRule(value, new Symbol[] { number });
			grammer.AddRule(value, new Symbol[] { string_ });

			////////////////////////////////////////
			//　start symbol
			////////////////////////////////////////
			grammer.SetStartSymbol(value);


			var resultCheck = grammer.CheckWhetherLL1OrNot();
			bool isLL1Grammer = resultCheck.Item1;
			var firstSet = resultCheck.Item2;
			var followSet = resultCheck.Item3;
			var epsilonDerivabilitySet = resultCheck.Item4;
			var directorSet = resultCheck.Item5;
			var overwrappingPairsDic = resultCheck.Item6;

			grammer.DisplayGrammer();
			DisplayFirstSet(firstSet);
			DisplayFollowSet(followSet);
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet);
			DisplayDirectorSet(directorSet);
			DisplayOverwrappingDirectorSets(overwrappingPairsDic);

			string result = isLL1Grammer ? "is" : "is NOT";
			string message = $"The specified grammer {result} a LL(1) grammer.";
			Console.WriteLine(message);
			return;
		}

		#region display

		private static void DisplayFirstSet(IDictionary<SymbolSequence, HashSet<Symbol>>? firstSet)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("FIRST SET");
			Console.WriteLine("----------------------------------------");
			if (firstSet is null)
			{
				Console.WriteLine("Failed to calculate FIRST SET");
			}
			else
			{
				foreach (var entry in firstSet)
				{
					string strValue = string.Join(" ", entry.Value);
					Console.WriteLine($"FIRST([{entry.Key}]) -> {{{strValue}}}");
				}
			}
			Console.WriteLine();
		}

		private static void DisplayFollowSet(IDictionary<Symbol, HashSet<Symbol>>? followSet)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("FOLLOW SET");
			Console.WriteLine("----------------------------------------");
			if (followSet is null)
			{
				Console.WriteLine("Failed to calculate FOLLOW SET");
			}
			else
			{
				foreach (var entry in followSet)
				{
					string strValue = string.Join(" ", entry.Value);
					Console.WriteLine($"FOLLOW({entry.Key}) -> {{{strValue}}}");
				}
			}
			Console.WriteLine();
		}

		private static void DisplayEpsilonDerivabilitySet(IDictionary<SymbolSequence, bool>? epsilonDerivabilitySet)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("ε-Derivability SET");
			Console.WriteLine("----------------------------------------");
			if (epsilonDerivabilitySet is null)
			{
				Console.WriteLine("Failed to calculate ε-Derivability SET");
			}
			else
			{
				foreach (var entry in epsilonDerivabilitySet)
				{
					Console.WriteLine($"εDerivability([{entry.Key}]) -> {entry.Value}");
				}
			}
			Console.WriteLine();
		}

		private static void DisplayDirectorSet(IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>? directorSet)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("DIRECTOR SET");
			Console.WriteLine("----------------------------------------");
			if (directorSet is null)
			{
				Console.WriteLine("Failed to calculate DIRECTOR SET");
			}
			else
			{
				foreach (var entry1 in directorSet)
				{
					foreach (var entry2 in entry1.Value)
					{
						string strSet = string.Join(" ", entry2.Value);
						Console.WriteLine($"DIRECTOR({entry1.Key},[{entry2.Key}]) -> {{{strSet}}}");
					}
				}
			}
			Console.WriteLine();
		}

		private static void DisplayOverwrappingDirectorSets(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>>? overwrappingPairsDic)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("Overwrapping DIRECTOR SET");
			Console.WriteLine("----------------------------------------");
			if (overwrappingPairsDic is null)
			{

			}
			else if (overwrappingPairsDic.Count <= 0)
			{
				Console.WriteLine("Theres is no overwrapping Director set");
				Console.WriteLine();
				return;
			}
			else
			{
				foreach (var entry in overwrappingPairsDic)
				{
					Symbol param1 = entry.Key;
					foreach (var pair in entry.Value)
					{
						string strSet1 = string.Join(" ", pair.Item1.Value);
						Console.WriteLine($"DIRECTOR({param1}, [{pair.Item1.Key}]) -> {{{strSet1}}}");
						string strSet2 = string.Join(" ", pair.Item2.Value);
						Console.WriteLine($"DIRECTOR({param1}, [{pair.Item2.Key}]) -> {{{strSet2}}}");
					}
				}
			}
			Console.WriteLine();
		}

		#endregion
	}
}
