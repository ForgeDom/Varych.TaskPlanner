#include <iostream>
#include <math.h>
#include <iomanip>

using namespace std;

double fx(double a)
{
    return pow(a, 3) - 4 * pow(a, 2) - 7 * a + 13;
}

double d1x(double a)
{
    return 3 * pow(a, 2) - 8 * a - 7;
}

double d2x(double a)
{
    return 6 * a - 8;
}

bool checkRoot(double a, double b)
{
    return (fx(a) * fx(b)) < 0;
}

int main()
{
    double a, b;
    cin >> a >> b;
    cout << "Let's check if there's a root in [" << a << "; " << b << "]:" << '\n';

    if (checkRoot(a, b))
    {
        cout << "There's a root!" << '\n' << '\n';
    }
    else
    {
        cout << "There's no root!" << '\n';
        exit(0);
    }

    cout << "Let's check the moving point: " << '\n';
    if ((fx(a) * d2x(a) > 0) && (fx(b) * d2x(b) < 0))
    {
        cout << "Moving point -> B" << '\n' << '\n';
        int counter = 1;
        while (checkRoot(a, b))
        {
            cout << '\n' << counter << " iteration:" << '\n';
            double prev_b = b;
            b = b - ((a - b) / (fx(a) - fx(b)) * fx(b));
            cout << "B = " << setprecision(20) << b << '\n';
            cout << "checking if there's still a root in between " << setprecision(2) << a << " and " << setprecision(20) << b << "..." << '\n';
            counter += 1;
            if (!checkRoot(a, b))
            {
                cout << "No, " << prev_b << " is the root." << '\n';
            }
            else cout << "Yes, continuing.." << '\n';
        }
        cout << "Precision:" << '\n';
        double pres = fx(b) / d1x(b);
        cout << abs(pres) << '\n';
    }
    if ((fx(b) * d2x(b) > 0) && (fx(a) * d2x(a) < 0))
    {
        cout << "Moving point -> A" << '\n' << '\n';
        int counter = 1;
        while (checkRoot(a, b))
        {
            cout << counter << " iteration:" << '\n';
            double prev_a = a;
            a = a - ((b - a) / (fx(b) - fx(a)) * fx(a));
            cout << "A = " << setprecision(8) << a << '\n';
            cout << "checking if there's still a root in between " << a << " and " << b << "..." << '\n';
            counter += 1;
            if (!checkRoot(a, b))
            {
                cout << "No, " << prev_a << " is the root." << '\n';
            }
            else cout << "Yes, continuing.." << '\n';
        }
        cout << "Precision:" << '\n';
        double pres = fx(a) / d1x(a);
        cout << setprecision(20) << pres << '\n';
    }

    return 0;
}
