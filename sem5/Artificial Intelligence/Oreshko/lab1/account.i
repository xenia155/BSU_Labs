% Copyright

interface account
    open core

predicates
    ssN : (string SocialSecurity [out]).
    setSocialSecurity : (string SSN).
    deposit : (real).
    setPassword : (string Password).
    setName : (string Name).
    setDepositNumber : (string DepositNumber).
    getBalance : (real Balance [out]).
    withdraw : (real Amount).
    transfer : (real Amount, account ToAccount).
    getPassword : (string Pwd [out]).
    getName : (string N [out]).
    getDepositNumber : (string DepNum [out]).

end interface account
