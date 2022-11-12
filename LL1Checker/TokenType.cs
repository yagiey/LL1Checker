namespace LL1Checker
{
	public enum TokenType
	{
		PunctuationLeftParen,
		PunctuationRightParen,
		PunctuationPlus,
		PunctuationMinus,
		PunctuationAsterisk,
		PunctuationSlash,
		LiteralInteger,
	}

	public enum TokenTypePL0p
	{
		/// <summary>integer number</summary>
		LiteralInteger,
		/// <summary>identifier</summary>
		Identifer,

		/// <summary>begin</summary>
		KeywordBegin,
		/// <summary>const</summary>
		KeywordConst,
		/// <summary>do</summary>
		KeywordDo,
		/// <summary>end</summary>
		KeywordEnd,
		/// <summary>function</summary>
		KeywordFunction,
		/// <summary>if</summary>
		KeywordIf,
		/// <summary>return</summary>
		KeywordReturn,
		/// <summary>then</summary>
		KeywordThen,
		/// <summary>var</summary>
		KeywordVar,
		/// <summary>while</summary>
		KeywordWhile,

		/// <summary>;</summary>
		PunctuationSemicolon,
		/// <summary>.</summary>
		PunctuationPeriod,
		/// <summary>,</summary>
		PunctuationComma,
		/// <summary>(</summary>
		PunctuationLeftParen,
		/// <summary>)</summary>
		PunctuationRightParen,
		/// <summary>:=</summary>
		PunctuationColonEqual,
		/// <summary>=</summary>
		PunctuationEqual,
		/// <summary><></summary>
		PunctuationLTGT,
		/// <summary>&lt;</summary>
		PunctuationLT,
		/// <summary>&lt;=</summary>
		PunctuationGT,
		/// <summary>&gt;</summary>
		PunctuationLE,
		/// <summary>&gt;=</summary>
		PunctuationGE,
		/// <summary>+</summary>
		PunctuationPlus,
		/// <summary>-</summary>
		PunctuationMinus,
		/// <summary>*</summary>
		PunctuationAsterisk,
		/// <summary>/</summary>
		PunctuationSlash,

		NameWrite,
		NameWriteln,
		NameOdd,
	}
}
