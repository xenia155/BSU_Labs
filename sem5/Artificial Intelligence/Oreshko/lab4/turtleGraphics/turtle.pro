% Copyright

implement turtle
    open core, vpiDomains, vpi, math

class facts
    turtle : (pnt, real) single.

clauses
    turtle(pnt(80, 80), -pi / 2).
    forward(Win, L) :-
        turtle(P1, Facing),
        P1 = pnt(X1, Y1),
        X2 = math::round(X1 + L * cos(Facing)),
        Y2 = math::round(Y1 + L * sin(Facing)),
        P2 = pnt(X2, Y2),
        drawline(Win, P1, P2),
        assert(turtle(P2, Facing)).

    move(L) :-
        turtle(P1, Facing),
        P1 = pnt(X1, Y1),
        X2 = round(X1 + L * cos(Facing)),
        Y2 = round(Y1 + L * sin(Facing)),
        P2 = pnt(X2, Y2),
        assert(turtle(P2, Facing)).
    right(A) :-
        turtle(P1, Facing),
        NewAngle = Facing + A,
        assert(turtle(P1, NewAngle)).
    left(A) :-
        turtle(P1, Facing),
        NewAngle = Facing - A,
        assert(turtle(P1, NewAngle)).

end implement turtle
