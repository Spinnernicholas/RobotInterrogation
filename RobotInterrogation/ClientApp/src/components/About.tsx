import * as React from 'react';
import { Link } from 'react-router-dom';
import { inCompatibilityMode, setCompatibilityMode } from 'src/Connectivity';
import { useState } from 'react';
import { Typography, Button, Link as A, makeStyles } from '@material-ui/core';
import { ActionSet } from './interviewParts/elements/ActionSet';
import { Page } from './interviewParts/elements/Page';
import { P } from './interviewParts/elements/P';

const useStyles = makeStyles(theme => ({
    compatibility: {
        marginTop: '1.5em',
    },
}));

export const About: React.FunctionComponent = () => {
    const [toggle, setToggle] = useState(false);

    const toggleCompatibility = () => {
        setCompatibilityMode(!inCompatibilityMode());
        setToggle(!toggle);
    }

    const toggleText = inCompatibilityMode()
        ? 'Disable compatibility mode'
        : 'Enable compatibility mode';

    const classes = useStyles();

    return (
        <Page>
            <Typography variant="h2" gutterBottom>Robot Interrogation</Typography>

            <P>This game is a conversation between two players, an Interviewer and a Suspect. It should either by two people in the same room, or over third-party video chat.</P>

            <P>The Suspect must convince the Interviewer that they are human. The Interviewer must determine whether they are a robot. Robots have strange personality quirks, but then so do humans under pressure...</P>

            <P>This is an online version of <A href="https://robots.management" target="_blank">Inhuman Conditions</A>, a game by Tommy Maranges and Cory O'Brien which is available for free under <A href="">Creative Commons license BY-NCA-SA-4.0</A>.</P>
            
            <P>Inhuman Conditions' <A href="https://www.dropbox.com/s/9ledq11mc3nd15f/Inhuman%20Conditions%20Rulebooks%20%28Public%20File%29.pdf?dl=0" target="_blank">rules are available here</A>. Read them before you play.</P>

            <P>If you're interested, you can <A href="https://github.com/FTWinston/RobotInterrogation" target="_blank">view the source</A> of this project on GitHub. Report any problems there.</P>

            <ActionSet>
                <Button variant="outlined" component={Link} to="/">Go back</Button>
            </ActionSet>

            <Typography className={classes.compatibility}>
                Connection problems?&nbsp;
                <Button color="secondary" onClick={toggleCompatibility}>{toggleText}</Button>
            </Typography>
        </Page>
    )
}
