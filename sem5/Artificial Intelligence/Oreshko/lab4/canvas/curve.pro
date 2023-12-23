% Copyright

implement curve
    open core, vpiDomains, math

class predicates
    star : (windowHandle, integer, real, integer).
clauses
    star(_Win, 0, _A, _L) :-
        !.
    star(Win, N, A, L) :-
        turtle::right(A),
        turtle::forward(Win, L),
        star(Win, N - 1, A, L).
    drawCurve(Win) :-
        star(Win, 10, 3 * pi / 5, 40).

end implement curve
