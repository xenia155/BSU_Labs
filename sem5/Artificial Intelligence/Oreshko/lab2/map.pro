% Copyright

implement map inherits formWindow
    open core, vpiDomains

clauses
    display(Parent) = Form :-
        Form = new(Parent),
        Form:show().

clauses
    new(Parent) :-
        formWindow::new(Parent),
        generatedInitialize().

predicates
    onMinskClick : button::clickResponder.
clauses
    onMinskClick(S) = button::defaultAction() :-
        Parent = S:getParent(),
        P = Parent:getVPIWindow(),
        draw::drawThem(P, "Минск").

predicates
    onBrestClick : button::clickResponder.
clauses
    onBrestClick(S) = button::defaultAction() :-
        Parent = S:getParent(),
        P = Parent:getVPIWindow(),
        draw::drawThem(P, "Брест").

predicates
    onGomelClick : button::clickResponder.
clauses
    onGomelClick(S) = button::defaultAction() :-
        Parent = S:getParent(),
        P = Parent:getVPIWindow(),
        draw::drawThem(P, "Гомель").

predicates
    onPushButtonClick : button::clickResponder.
clauses
    onPushButtonClick(Source) = _ :-
        Parent = Source:getParent(),
        P = Parent:getVPIWindow(),
        InputText = edit_ctl:getText(),
        %Position = edit_ctl:getPosition(),
        [XCoord, YCoord] = string::split(InputText, ","),
        XStr = string::trim(XCoord),
        YStr = string::trim(YCoord),
        X = toTerm(integer, XStr),
        Y = toTerm(integer, YStr),
        Point = pnt(X, Y),
        draw::drawName(P, Point, edit_ctl),
        fail.
    onPushButtonClick(_Source) = button::defaultAction().

% This code is maintained automatically, do not update it manually.
facts
    edit_ctl : editControl.
    pushButton_ctl : button.

predicates
    generatedInitialize : ().
clauses
    generatedInitialize() :-
        setText("map"),
        setRect(vpiDomains::rct(50, 40, 478, 313)),
        setDecoration(titlebar([frameDecoration::maximizeButton, frameDecoration::minimizeButton, frameDecoration::closeButton])),
        setBorder(frameDecoration::sizeBorder),
        setState([wsf_ClipSiblings, wsf_ClipChildren]),
        menuSet(noMenu),
        Minsk = button::new(This),
        Minsk:setText("Минск"),
        Minsk:setPosition(168, 60),
        Minsk:setSize(56, 16),
        Minsk:defaultHeight := false,
        Minsk:setAnchors([control::right, control::bottom]),
        Minsk:setClickResponder(onMinskClick),
        Brest = button::new(This),
        Brest:setText("Брест"),
        Brest:setPosition(84, 170),
        Brest:setSize(56, 16),
        Brest:defaultHeight := false,
        Brest:setAnchors([control::right, control::bottom]),
        Brest:setClickResponder(onBrestClick),
        Gomel = button::new(This),
        Gomel:setText("Гомель"),
        Gomel:setPosition(244, 166),
        Gomel:setSize(56, 16),
        Gomel:defaultHeight := false,
        Gomel:setAnchors([control::right, control::bottom]),
        Gomel:setClickResponder(onGomelClick),
        edit_ctl := editControl::new(This),
        edit_ctl:setText("Edit"),
        edit_ctl:setPosition(184, 118),
        pushButton_ctl := button::new(This),
        pushButton_ctl:setText("Check"),
        pushButton_ctl:setPosition(236, 118),
        pushButton_ctl:setWidth(56),
        pushButton_ctl:setClickResponder(onPushButtonClick).
% end of automatic code

end implement map
