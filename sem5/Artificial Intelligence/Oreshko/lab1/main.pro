% Copyright

implement main
    open core

clauses
    run() :-
        console::init(),
        A = account::new(),
        A:setSocialSecurity("AA345"),
        A:setName("Alex"),
        A:setPassword("1111"),
        A:setDepositNumber("5678-ABCD-EFGH-1234"),
        stdio::write("Name: "),
        A:getName(N),
        stdio::write(N),
        stdio::nl,
        stdio::write("SSN: "),
        A:ssN(SocialSecurity),
        stdio::write(SocialSecurity),
        stdio::nl,
        stdio::write("Password: "),
        A:getPassword(Pwd),
        stdio::write(Pwd),
        stdio::nl,
        stdio::write("Deposit number: "),
        A:getDepositNumber(DepNum),
        stdio::write(DepNum),
        stdio::nl,
        stdio::write("Balance: "),
        A:getBalance(Balance),
        stdio::write(Balance),
        stdio::nl,
        stdio::nl,
        A:deposit(5.0),
        A:getBalance(Balance2),
        stdio::write("Balance A after deposit: "),
        stdio::write(Balance2),
        stdio::nl,
        A:withdraw(2.0),
        A:getBalance(Balance3),
        stdio::write("Balance A after withdraw: "),
        stdio::write(Balance3),
        stdio::nl,
        B = account::new(),
        B:getBalance(Balance4),
        stdio::write("Balance B: "),
        stdio::write(Balance4),
        stdio::nl,
        A:transfer(2.0, B),
        A:getBalance(Balance5),
        stdio::write("Balance A after transfer: "),
        stdio::write(Balance5),
        stdio::nl,
        B:getBalance(Balance6),
        stdio::write("Balance B after transfer: "),
        stdio::write(Balance6).

end implement main

goal
    console::runUtf8(main::run).
