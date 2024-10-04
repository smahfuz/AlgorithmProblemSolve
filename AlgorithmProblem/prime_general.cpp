//find all prime number up to given n

#include<bits/stdc++.h>
using namespace std;

void fun(int n)
{
    int kk=1,flag=0;
   for(int i=1; i<=n; i++){
    flag=0;
    if(i<3){
        cout<<"number "<<kk<<" prime number is :" <<i<<endl;
        kk++;

    }


    else {


        for(int j=2; j<sqrt(i); j++){

            if(i%j==0){
                flag=1;

                break;
            }
        }
        if(flag == 0) {
            cout<<"number "<<kk<<" prime number is :" <<i<<endl;
            kk++;
        }
    }
   }
}

int main()
{
    int x; cin>>x;
    fun(x);
    return 0;
}
