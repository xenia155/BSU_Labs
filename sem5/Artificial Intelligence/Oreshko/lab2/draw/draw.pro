implement draw
    open core, vpiDomains, vpi

class facts
    city : (string, pnt).

clauses
    city("Минск", pnt(30, 40)).
    city("Брест", pnt(100, 120)).
    city("Гомель", pnt(100, 80)).
    city("Могилев", pnt(200, 100)).
    drawThem(Win, Name) :-
        B = brush(pat_solid, color_red),
        winSetBrush(Win, B),
        city(Name, P),
        !,
        P = pnt(X1, Y1),
        X2 = X1 + 20,
        Y2 = Y1 + 20,
        drawEllipse(Win, rct(X1, Y1, X2, Y2)).
    drawThem(_Win, _Name).
    drawName(Win, P, EditCtrl) :-
        B = brush(pat_solid, color_red),
        winSetBrush(Win, B),
        city(Name, P),
        !,
        EditCtrl:setText(Name).
    drawName(_Win, _P, _edit_ctl).

end implement draw
