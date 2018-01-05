/*
  Blink
  Turns on an LED on for one second, then off for one second, repeatedly.

  Most Arduinos have an on-board LED you can control. On the Uno and
  Leonardo, it is attached to digital pin 13. If you're unsure what
  pin the on-board LED is connected to on your Arduino model, check
  the documentation at http://www.arduino.cc

  This example code is in the public domain.

  modified 8 May 2014
  by Scott Fitzgerald
*/
int frdL = 8;
int bakL = 9;
int frdR = 10;
int bakR = 11;
char inByte;
// the setup function runs once when you press reset or power the board
void setup() {
  // initialize digital pin 13 as an output.
  Serial.begin(9600);
  pinMode(frdL, OUTPUT);
  pinMode(bakL, OUTPUT);
  pinMode(frdR, OUTPUT);
  pinMode(bakR, OUTPUT);
}

// the loop function runs over and over again forever
void loop() {
//    frd();
  //  delay(4000);
//    bak();
  //  delay(4000);
//    lft();
  //  delay(4000);
//    rit();
  //  delay(4000);
  if (Serial.available()) {
    inByte = Serial.read();
  }
  else
  {
    inByte='s';
    }
  switch (inByte)
  {
    case 'f':
      frd();
      delay(200);
      break;
    case 'r':
      rit();
      delay(200);
      break;
    case 'l':
      lft();
      delay(200);
      break;
    case 'b':
      bak();
      delay(200);
      break;
    case 's':
      stp();
//      delay(100);
      break;
    default:
     stp();
      break;
  }
}

void frd()
{
  digitalWrite(frdL, HIGH);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakL, LOW);    // turn the LED off by making the voltage LOW
  digitalWrite(frdR, HIGH);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakR, LOW);    // turn the LED off by making the voltage LOW

}

void bak()
{
  digitalWrite(frdL, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakL, HIGH);    // turn the LED off by making the voltage LOW
  digitalWrite(frdR, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakR, HIGH);    // turn the LED off by making the voltage LOW

}

void lft()
{
  digitalWrite(frdL, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakL, HIGH);    // turn the LED off by making the voltage LOW
  digitalWrite(frdR, HIGH);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakR, LOW);    // turn the LED off by making the voltage LOW

}

void rit()
{
  digitalWrite(frdL, HIGH);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakL, LOW);    // turn the LED off by making the voltage LOW
  digitalWrite(frdR, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakR, HIGH);    // turn the LED off by making the voltage LOW

}
void stp()
{
  digitalWrite(frdL, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakL, LOW);    // turn the LED off by making the voltage LOW
  digitalWrite(frdR, LOW);   // turn the LED on (HIGH is the voltage level)

  digitalWrite(bakR, LOW);    // turn the LED off by making the voltage LOW

}
