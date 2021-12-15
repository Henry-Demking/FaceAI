#include <iostream>
#include <fstream>
#include <GL/GLUT.h>
#include "Matrix.h"
#include "Quaternion.h"
#include "Splines.h"
#include <assert.h>
#include <math.h>
#include "Models.h"

#define PI 3.14159265358979




using namespace std;



// global variables

// screen size
int g_screenWidth  = 0;
int g_screenHeight = 0;

// frame index
int g_frameIndex = 0;

// angle for rotation
int g_angle = 30;

// position
float x=0;
float y=0;
float z=0;

// position in last frame
float px=0;
float py=0;
float pz=0;


// keyframe identification
GLint key=0;
GLint maxKey=7;
bool keyInc=true; // see if the key is increasing


Basketball* ball;
Basketball* ball2;
Basketball* ball3;
Basketball* ball4;
Basketball* ball5;

Wall* Floor;
Wall* ceiling;
Wall* leftWall;
Wall* rightWall;

Model* world; // abstract model, just use for collision detection

// init

void init( void ) {
    // init something before main loop...
    x=-2.0f;
    
    ball=new Basketball(0.25f,-1.0f,0.2f,-5.0f);
    ball->setMass(1.0f);
    ball->addForce(Quaternion(-0.1, 0, 10, 0,false)); // regular gravity
    ball->setVelocity(Vec(0.1,0.1,0));
    ball->setSpin(0, PI/10, 0);
    ball->setAccByForce();
    
    
    ball2=new Basketball(0.25f,1.0f,-0.0f,-5.0f);
    ball2->setMass(1.0f);
    ball2->addForce(Quaternion(-0.1, 0, 10, 0,false)); // regular gravity
    ball2->setVelocity(Vec(-0.1,0.1,0));
    ball2->setSpin(0, -PI/10, 0);
    ball2->setAccByForce();
    
    
    ball3=new Basketball(0.25f,0.0f,1.0f,-5.0f);
    
    ball3->setMass(1.0f);
    ball3->setVelocity(Vec(0,0,0));
    ball3->addForce(Quaternion(-0.1, 0, 10, 0,false)); // regular gravity
    ball3->setVelocity(Vec(0.06,-0.06,0));
    ball3->setSpin(0, PI/10, 0);
    ball3->setAccByForce();
    
    ball4=new Basketball(0.25f,-0.5f,0.8f,-5.0f);
    
    ball4->setMass(1.0f);
    ball4->addForce(Quaternion(-0.1, 0, 10, 0,false)); // regular gravity
    ball4->setVelocity(Vec(0.06,0.06,0));
    ball4->setSpin(0, -PI/10, 0);
    ball4->setAccByForce();

    ball5 = new Basketball(0.25f, 0.5f, 0.5f, -5.0f);

    ball5->setMass(1.0f);
    ball5->setVelocity(Vec(0, 0, 0));
    ball5->addForce(Quaternion(-0.1, 0, 10, 0, false)); // regular gravity
    ball5->setVelocity(Vec(0.06, -0.06, 0));
    ball5->setSpin(0, PI / 10, 0);
    ball5->setAccByForce();

 
    Floor=new Wall(3.0f,0.0f,-3.0f,-5.0f);
    ceiling=new Wall(3.0f,0.0f,3.0f,-5.0f);
    leftWall=new Wall(3.0f,-3.0f,0.0f,-5.0f);
    rightWall=new Wall(3.0f,3.0f,0.0f,-5.0f);
    
    world=new Model();
    // use hierarchial model for the container of all the models for collision detection
    // may implement hierarchial collision detection too
    world->addChild(*Floor);
    
    world->addChild(*ceiling);
    world->addChild(*leftWall);
    world->addChild(*rightWall);
    world->addChild(*ball);
    world->addChild(*ball2);
    world->addChild(*ball3);
    world->addChild(*ball4);
    world->addChild(*ball5);
    

}


// update

void update( void ) {
    // do something before rendering...
    
    // rotation angle
    g_angle = ( g_angle + 2 ) % 360;
    
    // switch keyframes
    if(g_angle % 360==0){
        
       
        
        if(keyInc){
            key++;
            if(key>=maxKey-3){
                key=0;
            }
        }
        
    }
    x+=0.005f;
    if(x>=2.0f){
        x=-2.0f;
    }
    
}



// render


// render help functions
void renderReady(){
    glClearColor (0.0, 1.0, 0.0, 0.0);
    glClearDepth (1.0);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    
    // render state
    glEnable(GL_DEPTH_TEST);
    glShadeModel(GL_SMOOTH);
    
    
    
    // modelview matrix
    glMatrixMode( GL_MODELVIEW );
    
    

    
}


void DrawObject(){
    
    ball->refresh(0.1f);
    ball->draw();
    
    
    ball2->refresh(0.1f);
    ball2->draw();
    
    ball3->refresh(0.1f);
    ball3->draw();

    
    ball4->refresh(0.1f);
    ball4->draw();


    ball5->refresh(0.1f);
    ball5->draw();
    
    Floor->draw();
    ceiling->draw();
    leftWall->draw();
    rightWall->draw();

    
    
}



void render( void ) {
    renderReady();
    
    DrawObject();
   
    
    glutSwapBuffers();
}





// reshape : update viewport and projection matrix when the window is resized
void reshape( int w, int h ) {
    // screen size
    g_screenWidth  = w;
    g_screenHeight = h;
    
    // viewport
    glViewport( 0, 0, (GLsizei)w, (GLsizei)h );
    
    // projection matrix
    glMatrixMode( GL_PROJECTION );
    glLoadIdentity();
    gluPerspective(45.0, (GLfloat)w/(GLfloat)h, 1.0, 2000.0);
}



// timer : triggered every 16ms ( about 60 frames per second )

void timer( int value ) {
    // increase frame index
    g_frameIndex++;
    
    update();
    
    // render
    glutPostRedisplay();
    
    // reset timer
    // 16 ms per frame ( about 60 frames per second )
    glutTimerFunc( 16, timer, 0 );
}

// main

int main( int argc, char** argv ) {
    // create opengL window
    glutInit( &argc, argv );
    glutInitDisplayMode( GLUT_DOUBLE | GLUT_RGB |GLUT_DEPTH );
    glutInitWindowSize( 600, 600 );
    glutInitWindowPosition( 100, 100 );
    glutCreateWindow( argv[0] );
    
    // init
    init();
    
    // set callback functions
    glutDisplayFunc( render );
    glutReshapeFunc( reshape );
    glutTimerFunc( 16, timer, 0 );
    
    // main loop
    glutMainLoop();
    
    return 0;
}