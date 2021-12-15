#ifndef Models_h
#define Models_h

#include "Quaternion.h"
#include "Splines.h"
#include "Matrix.h"
#include <GL/GLUT.h>
#include "Vector.h"

#define PI 3.14159265358979




// Hierachical Model class
class Model{
public:
    
    // create a model and put it into a specific location
    Model(float x=0,float y=0,float z=0,float rx=0, float ry=0,float rz=0,float qw=0,float qa=0,float qb=0,float qc=0 ){
        _x=x;
        _y=y;
        _z=z;
        _rx=rx;
        _ry=ry;
        _rz=rz;
        _qw=qw;
        _qa=qa;
        _qb=qb;
        _qc=qc;
        children.resize(0);
    }
    
    // use OpenGL function here
    void draw(){
        
        
    }
    void addChild(Model &m){
        children.push_back(&m);
        m.parent.push_back(this);
    }
    // rotate about the parent
    void rotate(float angle){
        
    }
    // self rotate, mode=0 is fixed angle, mode=1 is quaternion
    void selfRotate(int mode){
        if(mode==0){
            Quaternion r=Quaternion::fixedAngle(_rx,_ry , _rz);
            glMultMatrixf(r.rMatrix());
        }
        else if(mode==1){
            Quaternion r(_qw,_qa , _qb,_qc);
            glMultMatrixf(r.rMatrix());
            
        }
        
    }
    
    Model* getParent(){
        return parent[0];
    }
    
    
    
    
    
    
    vector<Model*> getDecendents(){
        vector<Model*> result;
        
        if(children.size()==0){
            //result.push_back(*this);
            
        }
        else{
            for(int i=0; i<children.size();i++){
                vector<Model*> d=children[i]->getDecendents();
                
                for(int j=0; j<d.size();j++){
                    result.push_back(d[j]);
                }
                result.push_back(children[i]);
                
            }
        }
        
        
        
        return result;
    }
    
    
    // physics functions
    //  Part 1: getters and setters

    float getMass(){
        return _mass;
    }
    void setMass(float mass){
        if(mass<=0){
            return;
        }
        else{
            _mass=mass;
        }
    }
    
    // the name are in pure physics concepts
    // velocity=speed vector, speed=magnitude
    Vec getVelocity(){
        return _speed;
    }
    float getSpeed(){
        return _speed.length();
    }
    void setVelocity(Vec v){
        _speed=v;
    }
    
    
    Vec getAcceleration(){
        return _acc;
    }
    float getAccMagitude(){
        return _acc.length();
    }
    void setAcceleration(Vec a){
        _acc=a;
    }
    
    void setSpin(float x, float y, float z){
        _omegax=x;
        _omegay=y;
        _omegaz=z;
    }
    

    //  Part 2: Force System

    
    void addForce(Quaternion f){
        _forces.push_back(f);
    }
    void addForce(Vec v){
        
    }
    // get the sum of forces in x, y and z direction
    Vec getAxisForces(){
        Vec result;
        float x=0;
        float y=0;
        float z=0;
        for(int i=0; i<_forces.size();i++){
            // the vertical angle=tan-1(z/(x^2+y^2)^(1/2))
            float theta=atanf((_forces[i]._c)/sqrtf(_forces[i]._a*_forces[i]._a+_forces[i]._b*_forces[i]._b));
            // the horizontal angle=tan-1(y/x)
            if(isnan(theta)){
                theta=PI/2;
            }
            float phi=atanf(_forces[i]._b/_forces[i]._a);
            if((isnan(phi))){
                
                phi=PI/2;
                
            }
            x+=_forces[i]._w*cosf(theta)*cosf(phi);
            y+=_forces[i]._w*cosf(theta)*sinf(phi);
            z+=_forces[i]._w*sinf(theta);
        }
        result.set(x,y,z);
        return result;
    }
    // set the acceleration according to force
    void setAccByForce(){
        Vec f=getAxisForces();
        _acc=f/_mass; // -F-> = m -a->
    }
    

    //  Part 3: Numerical Integration

    
    
    void refresh(float dt=1.0f){
        setNewVelocity(dt);
        setNewPosition(dt);
        setNewAngle(dt);
        if(checkCollide()){
            Vec newSpeed;
            newSpeed.set(_speed.x(),_speed.y()*0.9f,_speed.z()); // if collide, inverse the motion, the -0.9 is the estimation of energy loss
           
           
            _speed=_speed*(-1.0f);
            // a very simple approximation
            _omegax=-_omegax;
            _omegay=-_omegay;
            _omegaz=-_omegaz;
        
            
            setNewPosition(dt);
            setNewAngle(dt);
        
        }
       
    }
    
    // "integrate" acceleration to get velocity
    void setNewVelocity(float dt=1.0f){
        _speed=_speed+_acc*dt;
    }
    
    // "integrate" velocity to get position
    void setNewPosition(float dt=1.0f){
        
        _x=_x+_speed.x()*dt;
        _y=_y+_speed.y()*dt;
        _z=_z+_speed.z()*dt;
    }
    
    void setNewAngle(float dt=1.0f){
        _rx=_rx+_omegax*dt;
        _ry=_ry+_omegay*dt;
        _rz=_rz+_omegaz*dt;
        
    }
    
   
    
    

    
    
    
    // position of the model
    float _x;
    float _y;
    float _z;
    // orientation using fixed angle
    float _rx;
    float _ry;
    float _rz;
    // angular velocity
    float _omegax;
    float _omegay;
    float _omegaz;
    // orientation using Quaternion
    float _qw;
    float _qa;
    float _qb;
    float _qc;
    
    
    float size;
    
protected:
    vector<Model*> parent;
    vector<Model*> children;
    
    float _radius;
    
    // physical properties
    float _mass;
    Vec _speed;
    Vec _acc; // acceleration
    vector<Quaternion> _forces; // since force consist of magitude of the force and the direction of force
    // it is best to be represented by a quaternion, in this case rotational rules does not apply
    
    // collision detection using bounding sphere
    bool checkCollide(){
        vector<Model*> checkable=parent[0]->children;
        for(int i=0; i<checkable.size();i++){
            if(checkable[i]!=this){
                Vec FOI=Vec(checkable[i]->_x,checkable[i]->_y,checkable[i]->_z);
                Vec here=Vec(_x,_y,_z);
                Vec distance=here-FOI;
                float dis=distance.length();
                // if distance of the center is less than the sum of radius, there must be intersection
                if(dis<_radius+checkable[i]->_radius/2){
                    return true;
                }
            }
        }
        return false;
        
    }
    // collsion detection using bounding cube
    bool checkCollideCube(){
        vector<Model*> checkable=parent[0]->children;
        for(int i=0; i<checkable.size();i++){
            if(checkable[i]!=this){
                Vec min;
                float size=checkable[i]->size;
                min.set(checkable[i]->_x-size,checkable[i]->_y-size,checkable[i]->_z-size);
                Vec max;
                max.set(checkable[i]->_x+size,checkable[i]->_y+size,checkable[i]->_z+size);
                if(_x+this->size>=min.x()){
                    
                    
                    if(_x-this->size<=max.x()){
                        
                        
                        if(_y+this->size>=min.y()){
                            
                            if(_y-this->size<=max.y()){
                                
                                
                                
                                if(_z+this->size>=min.z()){
                                    
                                    
                                    
                                    if(_z-this->size<=max.z()){
                                        return true;
                                    }
                                    
                                    
                                    
                                }
                            }
                        }
                    }
                }
                
            }
        }
            return false;
    }
            
};

// this is not a good idea to use bounding sphere
class Wall : public Model{
public:
    Wall(float sizee=0.5f,float x=0.0f,float y=0.0f,float z=-5.0f){
        size=sizee;
        _x=x;
        _y=y;
        _z=z;
       
        _radius=size*sqrt(1.3);
    }
    
    
    void draw(int mode=0){
        glLoadIdentity();
        glTranslatef(_x, _y, _z);
        selfRotate(mode);
        glColor3f(0, 0, 1);
        glutSolidCube(size);
    }
};


class Basketball : public Model{
public:
    Basketball(float sizee=0.5f,float x=0.0f,float y=0.0f,float z=-5.0f,float ry=PI/2){
        size=sizee;
        _x=x;
        _y=y;
        _z=z;
        
        _ry=ry;
        
        _radius=size;
    }
   
    
    void draw(int mode=0){
        glLoadIdentity();
        glTranslatef(_x, _y, _z);
        selfRotate(mode);
        glColor3f(0, 0, 1);
        glutWireSphere(size,18,18);
    }
};


#endif /* Header_h */
