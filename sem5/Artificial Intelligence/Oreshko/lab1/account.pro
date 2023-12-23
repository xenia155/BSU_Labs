% Copyright

implement account
    open core

facts - customer
    funds : real := 3.0.
    personalData : (string Name, string SocialSecurity, string Password, string DepositNumber) single.

clauses
    personalData("", "", "", "").
    ssN(SS) :-
        personalData(_, SS, _, _).
    setSocialSecurity(SSN) :-
        personalData(N, _, Pwd, DepNum),
        assert(personalData(N, SSN, Pwd, DepNum)).
    deposit(Value) :-
        funds := funds + Value.
    setPassword(NewPwd) :-
        personalData(N, SSN, _, DepNum),
        assert(personalData(N, SSN, NewPwd, DepNum)).
    setDepositNumber(DepNum) :-
        personalData(N, SSN, Pwd, _),
        assert(personalData(N, SSN, Pwd, DepNum)).
    setName(NewName) :-
        personalData(_, SSN, Pwd, DepNum),
        assert(personalData(NewName, SSN, Pwd, DepNum)).
    getBalance(Balance) :-
        Balance = funds.
    withdraw(Amount) :-
        funds := funds - Amount.
    transfer(Amount, ToAccount) :-
        withdraw(Amount),
        ToAccount:deposit(Amount).
    getName(N) :-
        personalData(N, _, _, _).
    getPassword(Pwd) :-
        personalData(_, _, Pwd, _).
    getDepositNumber(DepNum) :-
        personalData(_, _, _, DepNum).

end implement account
